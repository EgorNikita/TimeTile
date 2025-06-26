using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.Create;

public class CreateRoleEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/", Handle)
            .WithSummary("Creates a new Role")
            .WithRequestValidation<Request>();
    }

    private static async Task<Created<Result<Response>>> Handle(
        Request request,
        TimetileDbContext db,
        IInstitutionProvider institutionProvider,
        CancellationToken cancellationToken)
    {
        // Extract InstitutionId
        var institutionId = institutionProvider.GetInstitutionId();

        // Save role
        var permissions = await db.Permissions
            .AsNoTracking()
            .Where(p => request.PermissionsIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var role = new Role
        {
            Title = request.Title.Trim(),
            InstitutionId = institutionId,
            RoleToPermissions = permissions
                .Select(permission => new RoleToPermission
                {
                    PermissionId = permission.Id
                })
                .ToList()
        };

        await db.Roles.AddAsync(role, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        // Return result
        var response = new Response(role.Id, role.Title);

        var result = Result.Success(response);

        return TypedResults.Created($"{API.Endpoints.Routes.Roles}/{role.Id}", result);
    }

    public sealed record Request(
        string Title,
        List<int> PermissionsIds
    );

    private sealed record Response(
        int Id,
        string Title
    );
}