using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.GetBulk
{
    public class GetSubmissionsBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns submissions by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var submissions = await db.Submissions
                .AsNoTracking()
                .Where(s => request.Ids.Contains(s.Id))
                .Select(x => new Response(
                    x.Id,
                    x.AssignmentId,
                    x.StudentId,
                    x.GradeId,
                    x.Status.ToString(),
                    x.StudentNote,
                    x.Feedback,
                    x.SubmittedAt,
                    db.SubmissionsFiles.Any(sf => sf.SubmissionId == x.Id)
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(submissions);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

        private sealed record Response(
            int Id,
            int AssignmentId,
            int StudentId,
            int? GradeId,
            string Status,
            string? StudentNote,
            string? Feedback,
            DateTimeOffset? SubmittedAt,
            bool HasAttachments
        );
    }
}
