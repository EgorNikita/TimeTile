using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.AddPermission
{
    public class AddPermissionToRoleEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/{RoleId:int}/permissions/{PermissionId:int}", Handle)
                .WithSummary("Adds Permission to Role")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relationExists = await db.RolesPermissions
                .AnyAsync(x => x.RoleId == request.RoleId && x.PermissionId == request.PermissionId, cancellationToken);

            if (!relationExists)
            {
                var entity = new RoleToPermission
                {
                    RoleId = request.RoleId,
                    PermissionId = request.PermissionId
                };

                await db.RolesPermissions.AddAsync(entity, cancellationToken);
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
