using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.GetBulk
{
    public class GetAssignmentsBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns assignments by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var assignments = await db.Assignments
                .AsNoTracking()
                .Where(a => request.Ids.Contains(a.Id))
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.Description,
                    x.PublishedAt,
                    x.Deadline,
                    x.UploadAfterDeadline,
                    db.AssignmentsFiles.Any(af => af.AssignmentId == x.Id)
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(assignments);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

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
