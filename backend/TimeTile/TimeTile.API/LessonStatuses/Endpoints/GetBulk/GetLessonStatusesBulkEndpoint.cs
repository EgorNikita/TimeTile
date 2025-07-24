using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.GetBulk
{
    public class GetLessonStatusesBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns lesson statuses by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var lessonStatuses = await db.LessonStatuses
                .AsNoTracking()
                .Where(s => request.Ids.Contains(s.Id))
                .Select(x => new Response(
                    x.Id,
                    x.Description
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(lessonStatuses);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

        private sealed record Response(
            int Id,
            string Description
        );
    }
}
