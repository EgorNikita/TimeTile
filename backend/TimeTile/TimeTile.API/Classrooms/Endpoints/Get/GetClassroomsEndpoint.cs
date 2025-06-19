using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.Get
{
    public class GetClassroomsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
               .MapGet("/", Handle)
               .WithSummary("Returns a page of classrooms")
               .WithRequestValidation<Request>();
        }

        private static async Task<Results<Ok<Result<PagedList<Response>>>, JsonHttpResult<Result>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extracts institutionId
            var institutionId = httpContext.GetInstitutionId();

            var classrooms = await db.Classrooms
                .AsNoTracking()
                .Where(c => c.InstitutionId == institutionId)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(c => new Response(
                    c.Id,
                    c.Title,
                    c.Capacity,
                    c.InstitutionId,
                    c.ClassroomTypeId
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(classrooms);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        public sealed record Response(
            int Id,
            string Title,
            int Capacity,
            int InstitutionId,
            int ClassroomTypeId
        );
    }
}
