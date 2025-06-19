using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Http;

namespace TimeTile.API.LessonStatuses.Endpoints.Get
{
    public class GetLessonStatusesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
               .MapGet("/", Handle)
               .WithSummary("Returns a page of lesson statuses")
               .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extracts institutionId
            var institutionId = httpContext.GetInstitutionId();

            var lessonStatuses = await db.LessonStatuses
                .AsNoTracking()
                .Where(c => c.InstitutionId == institutionId)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response(
                    x.Id,
                    x.Description,
                    x.ArgbColor
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(lessonStatuses);

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
            string Description,
            int ArgbColor
        );
    }
}
