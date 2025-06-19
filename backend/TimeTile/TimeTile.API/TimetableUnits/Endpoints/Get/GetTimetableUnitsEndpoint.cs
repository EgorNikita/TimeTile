using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.TimetableUnits.Endpoints.Get
{
    public class GetTimetableUnitsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of timetable units")
                .WithRequestValidation<Request>();
        }

        private static async Task<Results<Ok<Result<PagedList<Response>>>, JsonHttpResult<Result>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var institutionId = httpContext.GetInstitutionId();

            // Form a final paged list
            var timetableUnits = await db.TimetableUnits
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.StartTime,
                    x.EndTime
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(timetableUnits);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int Id,
            string Title,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime
        );
    }
}
