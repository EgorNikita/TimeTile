using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using static TimeTile.API.Endpoints;

namespace TimeTile.API.Messages.Endpoints.Create
{
    public class CreateMessageEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Adds a new Message")
                .RequireUserId()
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            IFileService fileService,
            IUserProvider userProvider,
            IMessageNotificationService messageNotificationService,
            CancellationToken cancellationToken)
        {
            var userId = userProvider.GetUserId();

            var message = new Message
            {
                UserId = userId,
                CourseId = request.CourseId,
                Content = request.Content?.Trim(),
                SentAt = DateTimeOffset.UtcNow
            };

            var hasAttachments = request.Files?.Any();

            if (hasAttachments.HasValue && hasAttachments.Value)
            {
                await SaveMessageWithAttachments(
                    message,
                    request,
                    db,
                    fileService,
                    cancellationToken
                );
            }
            else
            {
                await db.Messages.AddAsync(message, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            // Notification is processed in other thread to not block the response
            _ = messageNotificationService.NotifyMessageCreated(message);

            // Return result
            var response = new Response(
                message.Id,
                message.UserId,
                message.CourseId,
                message.Content,
                message.SentAt,
                message.EditedAt,
                db.MessagesFiles
                    .Where(mf => mf.MessageId == message.Id)
                    .Select(mf => mf.File.FileGuid.ToString())
                    .ToArray()
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Messages}/{message.Id}", result);
        }

        private static async Task SaveMessageWithAttachments(
            Message message,
            Request request,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var savedFileIds = await fileService.SaveFiles(request.Files!, cancellationToken);

                message.MessageToFiles = savedFileIds.Select(id => new MessageToFile { FileId = id }).ToList();

                await db.Messages.AddAsync(message, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                foreach (var fileId in message.MessageToFiles.Select(mf => mf.FileId))
                {
                    await fileService.DeleteFilePhysically(fileId, cancellationToken);
                }

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        public sealed record Request
        {
            public int CourseId { get; set; }
            public string? Content { get; set; }
            public IFormFileCollection? Files { get; set; }
        }

        private sealed record Response(
            int Id,
            int UserId,
            int CourseId,
            string? Content,
            DateTimeOffset SentAt,
            DateTimeOffset? EditedAt,
            string[] FileUrls
        );
    }
}
