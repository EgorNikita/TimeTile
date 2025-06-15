using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.CreateRole;

public class CreateRoleEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/", Handle)
            .WithSummary("Creates a new Role")
            .WithRequestValidation<Request>();
    }

    private static async Task<Results<Created<Result<Response>>, NotFound<Result>, BadRequest<Result>, JsonHttpResult<Result>>> Handle(
        Request request,
        TimetileDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        // Extract InstitutionId
        var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);

        if (institutionResult.IsFailure)
            return TypedResults.Json(
                Result.Failure(institutionResult.Error),
                statusCode: StatusCodes.Status401Unauthorized
            );

        var institutionId = institutionResult.Data;

        // Check if already exists
        var duplicateCheckResult = await IsRoleTitleExists(
            request.Title, institutionId, db, cancellationToken);

        if (duplicateCheckResult.IsFailure)
            return TypedResults.BadRequest(duplicateCheckResult);

        // Save role
        var permissionsResult = await GetPermissionsAsync(
            request.PermissionsIds,
            db,
            cancellationToken);

        if (permissionsResult.IsFailure)
            return TypedResults.BadRequest(
                Result.Failure(permissionsResult.Error));

        var permissions = permissionsResult.Data!;

        var role = new Role
        {
            Title = request.Title,
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

        return TypedResults.Created($"/roles/{role.Id}", result);
    }

    private static async Task<Result> IsRoleTitleExists(
        string title,
        int institutionId,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        var existing = await db.Roles
            .AsNoTracking()
            .Where(r => r.InstitutionId == institutionId)
            .AnyAsync(r => r.DeletedAt == null && r.Title == title, cancellationToken);

        if (existing)
            return Result.Failure(
                Error.From($"Role with title '{title}' already exists.", "ROLE_TITLE_EXISTS")
            );

        return Result.Success();
    }

    private static async Task<Result<List<Permission>>> GetPermissionsAsync(
        List<int> permissionIds,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        var permissions = await db.Permissions
            .AsNoTracking()
            .Where(p => permissionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var foundIds = permissions.Select(p => p.Id).ToHashSet();
        var missingIds = permissionIds.Where(id => !foundIds.Contains(id)).ToList();

        if (missingIds.Count == 0) return Result.Success(permissions);

        var error = Error.From(
            $"Some permissions were not found: {string.Join(", ", missingIds)}",
            "PERMISSIONS_NOT_FOUND"
        );
        return Result.Failure<List<Permission>>(error);
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