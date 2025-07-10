using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.GetById
{
    public class GetSubmissionByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns Submission by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find Submission
            var submission = await db.Submissions
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            var response = new Response(
                submission.Id,
                submission.AssignmentId,
                submission.StudentId,
                submission.GradeId,
                submission.Status.ToString(),
                submission.StudentNote,
                submission.Feedback,
                db.SubmissionsFiles.Any(sf => sf.SubmissionId == request.Id)
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

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
