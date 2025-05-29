using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.CreateRole;

public class CreateRoleEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new Role")
        .WithRequestValidation<Request>();

    public sealed record Request(
        string Title,
        List<int> PermissionsIds
    );
    
    private sealed record Response(
        int Id,
        string Title
    );
    
    private static async Task<IResult> Handle(
        Request request, 
        TimetileDbContext database,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        if (await IsRoleTitleExists(request.Title, database, cancellationToken))
            return Results.BadRequest($"Role with title '{request.Title}' already exists.");
        
        var permissionsResult = await GetPermissionsAsync(
            request.PermissionsIds,
            database,
            cancellationToken);

        if (permissionsResult.IsFailure)
            return Results.BadRequest(new { Error = permissionsResult.Error.Message });
        
        var permissions = permissionsResult.Data!;
        
        if (!TryGetInstitutionId(claimsPrincipal, out var institutionId))
            return Results.Unauthorized();
        
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

        var saveResult = await SaveRoleAsync(database, role, cancellationToken);
        if (saveResult.IsFailure)
            return Results.StatusCode(500);
        
        var response = new Response(role.Id, role.Title);
        return Results.Ok(response);
    }
    
    private static async Task<Result> SaveRoleAsync(
        TimetileDbContext database, 
        Role role, 
        CancellationToken cancellationToken)
    {
        try
        {
            database.Roles.Add(role);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while saving new role");
            return Result.Failure(new Error("DatabaseError", "Failed to save role"));
        }
    }
    
    private static async Task<bool> IsRoleTitleExists(
        string title,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        return await db.Roles.AnyAsync(r => r.Title == title, cancellationToken);
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
            code: "PERMISSIONS_NOT_FOUND"
        );
        return Result.Failure<List<Permission>>(error);
    }
    
    private static bool TryGetInstitutionId(ClaimsPrincipal claimsPrincipal, out int institutionId)
    {
        institutionId = 0;
        var institutionIdClaim = claimsPrincipal.FindFirst(CustomClaimTypes.InstitutionId);
        if (institutionIdClaim != null && int.TryParse(institutionIdClaim.Value, out institutionId)) return true;
        
        Log.Error("Institution ID claim is missing or invalid");
        return false;
    }
}
