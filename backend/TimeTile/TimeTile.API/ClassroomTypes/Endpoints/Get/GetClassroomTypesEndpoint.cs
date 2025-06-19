using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.ClassroomTypes.Endpoints.Get
{
    public class GetClassroomTypesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of classroom types")
                .WithRequestValidation<Request>();
        }

        private static async Task<Results<Ok<Result<PagedList<Response>>>, JsonHttpResult<Result>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            IClassroomTypeService classroomTypeService,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var institutionId = httpContext.GetInstitutionId();

            // Form a final paged list
            var classroomTypes = await db.ClassroomTypes
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.Description,
                    classroomTypeService.GetIconUrl(x)
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(classroomTypes);

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
            string Description,
            string? IconUrl
        );
    }
}
