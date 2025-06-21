using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.RemovePermission
{
    public class RemovePermissionFromRoleEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapDelete("/{RoleId:int}/permissions/{PermissionId:int}", Handle)
                .WithSummary("Removes Permission from Role")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find Relation
            var entity = await db.RolesPermissions
                .FirstOrDefaultAsync(x => x.RoleId == request.RoleId && x.PermissionId == request.PermissionId, cancellationToken);

            // Delete if relation exists
            if (entity is not null)
            {
                db.RolesPermissions.Remove(entity);
                await db.SaveChangesAsync(cancellationToken);
            }

            var result = Result.Success();

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int RoleId,
            int PermissionId
        );
    }
}
