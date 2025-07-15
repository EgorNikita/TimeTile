using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Endpoints.Update
{
    public class UpdateMessageEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}", Handle)
                .WithSummary("Partial update of Message")
                .RequireUserId()
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>()
                .DisableAntiforgery();
        }

        private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromForm] RequestBody body,
            IFileService fileService,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var message = await db.Messages
                .Include(m => m.MessageToFiles)
                    .ThenInclude(mf => mf.File)
                .FirstAsync(a => a.Id == parameters.Id, cancellationToken);

            // Update
            var updateResult = await UpdateEntity(message, body, fileService, db, cancellationToken);

            if (updateResult.IsFailure)
            {
                return TypedResults.BadRequest(updateResult);
            }

            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                message.Id,
                message.UserId,
                message.CourseId,
                message.Content,
                message.SentAt,
                message.EditedAt,
                message.MessageToFiles
                    .Select(mf => mf.File.FileGuid.ToString())
                    .ToArray()
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        private static async Task<Result> UpdateEntity(
            Message message,
            RequestBody request,
            IFileService fileService,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var isUpdated = false;

            if (request.Content is not null)
            {
                isUpdated = request.Content.Trim() != message.Content;
                message.Content = request.Content.Trim();
            }

            if (request.Content == string.Empty)
            {
                isUpdated = message.Content != null;
                message.Content = null;
            }

            if (request.FilesToAdd is not null && request.FilesToAdd.Any())
            {
                await AddFiles(message.Id, request.FilesToAdd, fileService, db, cancellationToken);
                isUpdated = true;
            }

            if (request.FilesToRemove is not null && request.FilesToRemove.Any())
            {
                var removeFilesResult = await RemoveFiles(message, request.FilesToRemove, fileService, db, cancellationToken);

                if (removeFilesResult.IsFailure)
                {
                    return removeFilesResult;
                }

                isUpdated = removeFilesResult.Data > 0;
            }

            if (isUpdated)
            {
                message.EditedAt = DateTimeOffset.UtcNow;
            }

            return Result.Success();
        }

        private static async Task AddFiles(
           int id,
           IFormFileCollection files,
           IFileService fileService,
           TimetileDbContext db,
           CancellationToken cancellationToken)
        {
            var savedFileIds = await fileService.SaveFiles(files, cancellationToken);

            await db.MessagesFiles.AddRangeAsync(
                savedFileIds.Select(fileId =>
                    new MessageToFile { MessageId = id, FileId = fileId }
                ), cancellationToken
            );
        }

        private static async Task<Result<int>> RemoveFiles(
           Message message,
           List<int> fileIds,
           IFileService fileService,
           TimetileDbContext db,
           CancellationToken cancellationToken)
        {
            var relations = message.MessageToFiles
                .Where(mf => fileIds.Contains(mf.FileId))
                .ToList();

            if (relations.Count == message.MessageToFiles.Count && message.Content == null)
            {
                var error = Error.From("Message must have either content or files attached.");
                return Result.Failure<int>(error);
            }

            foreach (var relation in relations)
            {
                db.MessagesFiles.Remove(relation);
                db.Files.Remove(relation.File);
                await fileService.DeleteFilePhysically(relation.FileId, cancellationToken);
            }

            return Result.Success(relations.Count);
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody
        {
            public string? Content { get; set; }
            public IFormFileCollection? FilesToAdd { get; set; }
            public List<int>? FilesToRemove { get; set; }
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
