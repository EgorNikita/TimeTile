using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;
using Microsoft.EntityFrameworkCore;

namespace TimeTile.API.LessonStatuses.Endpoints.Get
{
    public class GetLessonStatusesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
               .MapGet("/", Handle)
               .WithSummary("Returns lesson statuses")
               .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var lessonStatuses = await db.LessonStatuses
                .AsNoTracking()
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response(
                    x.Id,
                    x.Description
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(lessonStatuses);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            string? SortBy = null,
            bool Descending = false
        ) : ISortRequest;

        private sealed record Response(
            int Id,
            string Description
        );
    }
}
