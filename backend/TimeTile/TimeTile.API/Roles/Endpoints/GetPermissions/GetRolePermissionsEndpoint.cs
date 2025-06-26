using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.GetPermissions
{
    public class GetRolePermissionsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{RoleId:int}/permissions", Handle)
                .WithSummary("Gets Permissions of the Role")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var permissions = await db.Permissions
                .AsNoTracking()
                .Where(p => p.RolesToPermission.Any(x => x.RoleId == request.RoleId))
                .Include(p => p.RolesToPermission)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(p => new Response(
                    p.Id,
                    p.Description
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(permissions);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int RoleId,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int Id,
            string Description
        );
    }
}
