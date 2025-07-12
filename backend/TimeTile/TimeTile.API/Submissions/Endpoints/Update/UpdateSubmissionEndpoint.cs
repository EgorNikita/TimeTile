using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Json;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Update
{
    public class UpdateSubmissionEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}", Handle)
                .WithSummary("Partial update of Submission")
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
            var submission = await db.Submissions
                .FirstAsync(a => a.Id == parameters.Id, cancellationToken);

            // Update SubmissionsToFiles
            if (body.FilesToAdd is not null)
            {
                await AddFiles(submission.Id, body.FilesToAdd, fileService, db, cancellationToken);
            }
            if (body.FilesToRemove is not null)
            {
                await RemoveFiles(submission.Id, body.FilesToRemove, fileService, db, cancellationToken);
            }

            // Update other fields
            await UpdateEntity(submission, body, db, cancellationToken);

            if (submission.Grade is not null && submission.Status != SubmissionStatus.Accepted)
            {
                var error = Error.From("Grade can be passed only if Submission status is Accepted.");
                var failure = Result.Failure(error);

                return TypedResults.BadRequest(failure);
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

        private static async Task UpdateEntity(
            Submission submission,
            RequestBody request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (request.Status is not null)
            {
                submission.Status = request.Status.Value;

                // If status is not Accepted, remove the grade if it exists
                if (submission.Status != SubmissionStatus.Accepted)
                {
                    if (submission.Grade is not null)
                    {
                        db.Grades.Remove(submission.Grade);
                        submission.Grade = null;
                    }
                }
            }

            if (request.GradeValue != null)
            {
                if (submission.Grade is not null)
                {
                    db.Grades.Remove(submission.Grade);
                }

                var grade = new Grade
                {
                    Value = request.GradeValue.Value,
                    Weight = request.GradeWeight!.Value,
                    Type = GradeType.Homework
                };

                await db.Grades.AddAsync(grade, cancellationToken);

                submission.Grade = grade;
            }

            if (request.StudentNote is not null)
            {
                submission.StudentNote = request.StudentNote.Trim();
            }

            if (request.Feedback is not null)
            {
                submission.Feedback = request.Feedback.Trim();
            }
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody
        {
            public short? GradeValue { get; set; }
            public float? GradeWeight { get; set; }
            public SubmissionStatus? Status { get; set; }
            public string? StudentNote { get; set; }
            public string? Feedback { get; set; }
            [FromForm(Name = "addFiles")] public IFormFileCollection? FilesToAdd { get; set; }
            [FromForm(Name = "removeFiles")] public List<int>? FilesToRemove { get; set; }
        }

        private sealed record Response(
            int Id,
            int AssignmentId,
            int StudentId,
            int? GradeId,
            string Status,
            string StudentNote,
            string Feedback,
            string[] FileUrls
        );
    }
}
