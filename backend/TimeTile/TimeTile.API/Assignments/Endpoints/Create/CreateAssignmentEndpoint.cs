using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeTile.API.Common.Api;
using static TimeTile.API.Endpoints;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Common.Interfaces.Services;
using Serilog.Parsing;
using System.Threading;
using TimeTile.API.Files.Services;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Assignments.Endpoints.Create
{
    public class CreateAssignmentEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Adds a new Assignment to Lesson")
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            var assignment = new Assignment
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                PublishedAt = request.PublishedAt.ToUniversalTime(),
                Deadline = request.Deadline.ToUniversalTime(),
                UploadAfterDeadline = request.UploadAfterDeadline,
                Lesson = await db.Lessons.FirstAsync(l => l.Id == request.LessonId, cancellationToken)
            };

            var hasAttachments = request.Files?.Any();

            if (hasAttachments.HasValue && hasAttachments.Value)
            {
                await SaveAssignmentWithAttachments(
                    assignment,
                    request,
                    db,
                    fileService,
                    cancellationToken
                );
            }
            else
            {
                await db.Assignments.AddAsync(assignment, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            // Return result
            var response = new Response(
                assignment.Id,
                assignment.Title,
                assignment.Description,
                assignment.PublishedAt,
                assignment.Deadline,
                assignment.UploadAfterDeadline,
                hasAttachments.HasValue && hasAttachments.Value
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Assignments}/{assignment.Id}", result);
        }

        private static async Task SaveAssignmentWithAttachments(
            Assignment assignment,
            Request request,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            List<int> savedFileIds = [];

            try
            {
                foreach (var file in request.Files)
                {
                    var savedFileId = await fileService.SaveFile(file.OpenReadStream(), file.FileName, cancellationToken);
                    savedFileIds.Add(savedFileId);
                }

                assignment.AssignmentToFiles = savedFileIds.Select(id => new AssignmentToFile { FileId = id }).ToList();

                await db.Assignments.AddAsync(assignment, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                foreach (var fileId in savedFileIds)
                {
                    await fileService.DeleteFilePhysically(fileId, cancellationToken);
                }

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        public sealed record Request
        {
            public int LessonId { get; set; }
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public DateTimeOffset PublishedAt { get; set; }
            public DateTimeOffset Deadline { get; set; }
            public bool UploadAfterDeadline { get; set; }
            public IFormFileCollection? Files { get; set; }
        }

        private sealed record Response(
            int Id,
            string Title,
            string Description,
            DateTimeOffset PublishedAt,
            DateTimeOffset Deadline,
            bool UploadAfterDeadline,
            bool HasAttachments
        );
    }
}
