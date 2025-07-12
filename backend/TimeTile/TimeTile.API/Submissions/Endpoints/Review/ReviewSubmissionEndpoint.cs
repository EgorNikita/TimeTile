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

namespace TimeTile.API.Submissions.Endpoints.Review
{
    public class ReviewSubmissionEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}/review", Handle)
                .WithSummary("Review Submission")
                .RequireUserId()
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var submission = await db.Submissions
                .Include(s => s.Grade)
                .FirstAsync(s => s.Id == parameters.Id, cancellationToken);

            if (body.Feedback.WasProvided)
            {
                submission.Feedback = body.Feedback.Value;
            }

            if (body.Grade.WasProvided)
            {
                if (submission.Grade is not null)
                {
                    db.Grades.Remove(submission.Grade);
                }

                var grade = body.Grade.Value;

                if (grade is null)
                {
                    submission.Grade = null;
                }
                else
                {
                    var gradeToAdd = new Grade
                    {
                        Value = grade.Value,
                        Weight = grade.Weight,
                        Type = GradeType.Homework
                    };

                    await db.Grades.AddAsync(gradeToAdd, cancellationToken);

                    submission.Grade = gradeToAdd;
                }
            }

            submission.Status = submission.Grade is null
                ? SubmissionStatus.Rejected
                : SubmissionStatus.Accepted;

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

        public sealed record RequestParameters
        {
            public int Id { get; set; }
        }

        public sealed record RequestBody(
            PatchOptionalProperty<GradeInfo?> Grade,
            PatchOptionalProperty<string?> Feedback
        );

        public sealed record GradeInfo(
            short Value,
            float Weight
        );

        private sealed record Response(
            int Id,
            int AssignmentId,
            int StudentId,
            int? GradeId,
            string Status,
            string? StudentNote,
            string? Feedback,
            string[] FileUrls
        );
    }
}
