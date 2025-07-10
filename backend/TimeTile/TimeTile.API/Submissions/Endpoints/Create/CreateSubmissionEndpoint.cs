using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using static TimeTile.API.Endpoints;

namespace TimeTile.API.Submissions.Endpoints.Create
{
    public class CreateSubmissionEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Adds a new Submission")
                .RequireUserId()
                .WithRequestValidation<Request>()
                .DisableAntiforgery();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromForm] Request request,
            TimetileDbContext db,
            IFileService fileService,
            IUserProvider userProvider,
            CancellationToken cancellationToken)
        {
            var userId = userProvider.GetUserId();

            var submission = new Submission
            {
                AssignmentId = request.AssignmentId,
                StudentId = userId,
                Grade = request.GradeValue != null ? new Grade
                {
                    Value = request.GradeValue!.Value,
                    Weight = request.GradeWeight!.Value,
                    Type = GradeType.Homework
                } : null,
                Status = request.Status,
                StudentNote = request.StudentNote.Trim(),
                Feedback = request.Feedback.Trim()
            };

            var hasAttachments = request.Files?.Any();

            if (hasAttachments.HasValue && hasAttachments.Value)
            {
                await SaveSubmissionWithAttachments(
                    submission,
                    request,
                    db,
                    fileService,
                    cancellationToken
                );
            }
            else
            {
                await db.Submissions.AddAsync(submission, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            // Return result
            var response = new Response(
                submission.Id,
                submission.AssignmentId,
                submission.StudentId,
                submission.GradeId,
                submission.Status.ToString(),
                submission.StudentNote,
                submission.Feedback,
                hasAttachments.HasValue && hasAttachments.Value
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Submissions}/{submission.Id}", result);
        }

        private static async Task SaveSubmissionWithAttachments(
            Submission submission,
            Request request,
            TimetileDbContext db,
            IFileService fileService,
            CancellationToken cancellationToken)
        {
            using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var savedFileIds = await fileService.SaveFiles(request.Files!, cancellationToken);

                submission.SubmissionToFiles = savedFileIds.Select(id => new SubmissionToFile { FileId = id }).ToList();

                await db.Submissions.AddAsync(submission, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                foreach (var fileId in submission.SubmissionToFiles.Select(sf => sf.FileId))
                {
                    await fileService.DeleteFilePhysically(fileId, cancellationToken);
                }

                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }

        public sealed record Request
        {
            public int AssignmentId { get; set; }

            [FromForm(Name = "GradeValue")]
            public short? GradeValue { get; set; }

            [FromForm(Name = "GradeWeight")]
            public float? GradeWeight { get; set; }

            public SubmissionStatus Status { get; set; }
            public string StudentNote { get; set; } = null!;
            public string Feedback { get; set; } = null!;
            public IFormFileCollection? Files { get; set; }
        }

        private sealed record Response(
            int Id,
            int AssignmentId,
            int StudentId,
            int? GradeId,
            string Status,
            string StudentNote,
            string Feedback,
            bool HasAttachments
        );
    }
}
