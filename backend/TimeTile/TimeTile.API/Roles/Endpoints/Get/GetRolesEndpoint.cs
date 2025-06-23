using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.Get
{
    public class GetRolesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of roles")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            // Form a final paged list
            var roles = await db.Roles
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(r => new Response
                (
                    r.Id,
                    r.Title
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(roles);

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
            string Title
        );
    }
}
