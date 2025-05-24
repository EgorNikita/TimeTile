using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
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
        {
            return Results.BadRequest($"Role with title '{request.Title}' already exists.");
        }
        
        var permissions = await database.Permissions
            .Where(p => request.PermissionsIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
        
        if (permissions.Count != request.PermissionsIds.Count)
        {
            return Results.BadRequest("Some permission IDs are invalid.");
        }
        
        var role = new Role
        {
            Title = request.Title,
            Permissions = permissions
        };

        try
        {
            database.Roles.Add(role);
            await database.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
        
        var response = new Response(role.Id, role.Title);
        return Results.Ok(response);
    }
    
    private static async Task<bool> IsRoleTitleExists(
        string title,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        return await db.Roles.AnyAsync(r => r.Title == title, cancellationToken);
    }
    
    // private static async Task<Result<List<Permission>>> GetPermissionsAsync(
    //     List<int> permissionIds,
    //     TimetileDbContext db,
    //     CancellationToken cancellationToken)
    // {
    //     var permissions = await db.Permissions
    //         .AsNoTracking()
    //         .Where(p => permissionIds.Contains(p.Id))
    //         .ToListAsync(cancellationToken);
    //
    //     return permissions.Count != permissionIds.Count
    //         ? Result<List<Permission>>.Invalid()
    //         : Result<List<Permission>>.Valid(permissions);
    // }
}
