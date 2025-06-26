using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.UpdatePermissions
{
    public class UpdatePermissionsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{RoleId:int}/permissions", Handle)
                .WithSummary("Updates Permission by Role")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (body.PermissionsToAdd is not null)
            {
                await AddPermissions(parameters.RoleId, body.PermissionsToAdd, db, cancellationToken);
            }
            if (body.PermissionsToRemove is not null)
            {
                await RemovePermissions(parameters.RoleId, body.PermissionsToRemove, db, cancellationToken);
            }

            var permissions = await db.Permissions
                .AsNoTracking()
                .Include(p => p.RolesToPermission)
                .Where(p => p.RolesToPermission.Any(x => x.RoleId == parameters.RoleId))
                .Select(p => new Response(
                    p.Id,
                    p.Description
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(permissions);

            return TypedResults.Ok(result);
        }

        private static async Task AddPermissions(
            int roleId, 
            List<int> permissionIds, 
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var existingPermissionIds = await db.RolesPermissions
                .Where(x => x.RoleId == roleId)
                .Select(x => x.PermissionId)
                .ToListAsync(cancellationToken);

            var permissionsToAdd = permissionIds
                .Where(permId => !existingPermissionIds.Contains(permId))
                .Select(permId => new Core.Models.RoleToPermission
                {
                    RoleId = roleId,
                    PermissionId = permId
                })
                .ToList();

            if (permissionsToAdd.Any())
            {
                await db.RolesPermissions.AddRangeAsync(permissionsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task RemovePermissions(
            int roleId,
            List<int> permissionIds,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var permissionsToRemove = await db.RolesPermissions
                .Where(x => x.RoleId == roleId && permissionIds.Contains(x.PermissionId))
                .ToListAsync(cancellationToken);

            if (permissionsToRemove.Any())
            {
                db.RolesPermissions.RemoveRange(permissionsToRemove);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public sealed record RequestParameters(
            int RoleId
        );

        public sealed record RequestBody(
            [property: JsonPropertyName("add")] List<int>? PermissionsToAdd,
            [property: JsonPropertyName("remove")] List<int>? PermissionsToRemove
        );

        private sealed record Response(
            int Id,
            string Description
        );
    }
}
