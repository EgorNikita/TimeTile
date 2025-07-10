using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Json;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.Update
{
    public class UpdateAssignmentEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}", Handle)
                .WithSummary("Partial update of Assignment")
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
            var assignment = await db.Assignments
                .FirstAsync(a => a.Id == parameters.Id, cancellationToken);

            // Update AssignmentsToFiles
            if (body.FilesToAdd is not null)
            {
                await AddFiles(assignment.Id, body.FilesToAdd, fileService, db, cancellationToken);
            }
            if (body.FilesToRemove is not null)
            {
                await RemoveFiles(assignment.Id, body.FilesToRemove, fileService, db, cancellationToken);
            }

            // Update other fields
            await UpdateEntity(assignment, body);

            if (assignment.PublishedAt >= assignment.Deadline)
            {
                var error = Error.From("PublishedAt should be less than Deadline.");
                var failure = Result.Failure(error);

                return TypedResults.BadRequest(failure);
            }

            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                assignment.Id,
                assignment.Title,
                assignment.Description,
                assignment.PublishedAt,
                assignment.Deadline,
                assignment.UploadAfterDeadline,
                db.AssignmentsFiles
                    .Where(af => af.AssignmentId == assignment.Id) 
                    .Select(af => af.File.FileGuid.ToString())
                    .ToArray()
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        private static async Task AddFiles(
           int id,
           IFormFileCollection files,
           IFileService fileService,
           TimetileDbContext db,
           CancellationToken cancellationToken)
        {
            var savedFileIds = await fileService.SaveFiles(files, cancellationToken);

            await db.AssignmentsFiles.AddRangeAsync(
                savedFileIds.Select(fileId => 
                    new AssignmentToFile { AssignmentId = id, FileId = fileId }
                ), cancellationToken
            );
        }

        private static async Task RemoveFiles(
           int id,
           List<int> fileIds,
           IFileService fileService,
           TimetileDbContext db,
           CancellationToken cancellationToken)
        {
            var relations = await db.AssignmentsFiles
                .Include(af => af.File)
                .Where(af => af.AssignmentId == id && fileIds.Contains(af.FileId))
                .ToListAsync(cancellationToken);

            foreach (var relation in relations)
            {
                db.AssignmentsFiles.Remove(relation);
                db.Files.Remove(relation.File);
                await fileService.DeleteFilePhysically(relation.FileId, cancellationToken);
            }
        }

        private static async Task UpdateEntity(
            Assignment assignment,
            RequestBody request)
        {
            if (request.Title is not null)
            {
                assignment.Title = request.Title.Trim();
            }

            if (request.Description is not null)
            {
                assignment.Description = request.Description.Trim();
            }

            if (request.PublishedAt is not null)
            {
                assignment.PublishedAt = request.PublishedAt.Value.ToUniversalTime();
            }

            if (request.Deadline is not null)
            {
                assignment.Deadline = request.Deadline.Value.ToUniversalTime();
            }

            if (request.UploadAfterDeadline is not null)
            {
                assignment.UploadAfterDeadline = request.UploadAfterDeadline.Value;
            }
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody
        {
            public string? Title { get; set; } = null!;
            public string? Description { get; set; } = null!;
            public DateTimeOffset? PublishedAt { get; set; }
            public DateTimeOffset? Deadline { get; set; }
            public bool? UploadAfterDeadline { get; set; }
            [FromForm(Name = "addFiles")] public IFormFileCollection? FilesToAdd { get; set; }
            [FromForm(Name = "removeFiles")] public List<int>? FilesToRemove { get; set; }
        }

        private sealed record Response(
            int Id,
            string Title,
            string Description,
            DateTimeOffset PublishedAt,
            DateTimeOffset Deadline,
            bool UploadAfterDeadline,
            string[] FileUrls
        );
    }
}
