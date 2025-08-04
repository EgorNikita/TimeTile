using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using TimeTile.API.Common.Api;
using TimeTile.Core.Common.Interfaces.Services;

namespace TimeTile.API.Auth.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            Log.Information($"Permission {requirement.Permission} denied - invalid user ID");
            return;
        }

        Log.Information($"Checking permission: {requirement.Permission} for user: {userId}");
        
        var hasPermission = await _permissionService.UserHasPermissionAsync(userId, requirement.Permission);

        if (hasPermission)
        {
            Log.Information($"Permission {requirement.Permission} granted for user: {userId}");
            context.Succeed(requirement);
            return;
        }

        PermissionFallbackHandler.Handle(userId, context, requirement);
    }
}