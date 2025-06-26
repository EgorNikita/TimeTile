using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Endpoints.GetPermissions
{
    public class GetUserPermissionsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{UserId:int}/permissions", Handle)
                .WithSummary("Gets Permissions of the User")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var roleId = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == request.UserId)
                .Select(u => u.RoleId)
                .FirstAsync(cancellationToken);

            var permissions = await db.Permissions
                .AsNoTracking()
                .Where(p => p.RolesToPermission.Any(x => x.RoleId == roleId))
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
            int UserId,
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
