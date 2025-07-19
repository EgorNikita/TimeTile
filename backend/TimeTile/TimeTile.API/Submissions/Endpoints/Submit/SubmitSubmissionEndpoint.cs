using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog.Parsing;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Submit
{
    public class SubmitSubmissionEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}/submit", Handle)
                .WithSummary("Submits a Submission")
                .RequireUserId()
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>()
                .RequireAuthorization(Permissions.Submissions.Submit)
                .DisableAntiforgery();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromForm] RequestBody body,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            var submission = await db.Submissions
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Lesson)
                .FirstAsync(s => s.Id == parameters.Id, cancellationToken);

            if (body.StudentNote != null)
            {
                submission.StudentNote = body.StudentNote != string.Empty
                    ? body.StudentNote.Trim()
                    : null;
            }

            submission.SubmittedAt = DateTimeOffset.UtcNow;

            submission.Status = submission.Assignment.Deadline >= submission.SubmittedAt
                    ? SubmissionStatus.Submitted
                    : SubmissionStatus.SubmittedLate;

            // Update SubmissionsToFiles
            if (body.FilesToAdd is not null)
            {
                await AddFiles(submission.Id, body.FilesToAdd, fileService, db, cancellationToken);
            }
            if (body.FilesToRemove is not null)
            {
                await RemoveFiles(submission.Id, body.FilesToRemove, fileService, db, cancellationToken);
            }

            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                submission.Id,
                submission.AssignmentId,
                submission.StudentId,
                submission.GradeId,
                submission.Status.ToString(),
                submission.StudentNote,
                submission.Feedback,
                submission.SubmittedAt,
                await db.SubmissionsFiles
                    .Where(sf => sf.SubmissionId == submission.Id)
                    .Select(sf => sf.File.FileGuid.ToString())
                    .ToArrayAsync(cancellationToken)
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

            await db.SubmissionsFiles.AddRangeAsync(
                savedFileIds.Select(fileId =>
                    new SubmissionToFile { SubmissionId = id, FileId = fileId }
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
            var relations = await db.SubmissionsFiles
                .Include(sf => sf.File)
                .Where(sf => sf.SubmissionId == id && fileIds.Contains(sf.FileId))
                .ToListAsync(cancellationToken);

            foreach (var relation in relations)
            {
                db.SubmissionsFiles.Remove(relation);
                db.Files.Remove(relation.File);
                await fileService.DeleteFilePhysically(relation.FileId, cancellationToken);
            }
        }

        public sealed record RequestParameters
        {
            public int Id { get; set; }
        }

        public sealed record RequestBody
        {
            public string? StudentNote { get; set; }
            public IFormFileCollection? FilesToAdd { get; set; }
            public List<int>? FilesToRemove { get; set; }
        }

        private sealed record Response(
            int Id,
            int AssignmentId,
            int StudentId,
            int? GradeId,
            string Status,
            string? StudentNote,
            string? Feedback,
            DateTimeOffset? SubmittedAt,
            string[] FileUrls
        );
    }
}
