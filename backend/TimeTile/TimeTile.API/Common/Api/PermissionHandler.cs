using Microsoft.AspNetCore.Authorization;
using Serilog;
using TimeTile.API.Common.Constants;

namespace TimeTile.API.Common.Api;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        Log.Information($"Checking permission: {requirement.Permission}");
        var hasPermission = context.User.HasClaim(c => c.Type == CustomClaimTypes.Permission
                                                       && c.Value == requirement.Permission);

        if (hasPermission)
        {
            Log.Information($"Permission {requirement.Permission} granted.");
            context.Succeed(requirement);
        }
        else
        {
            Log.Information($"Permission {requirement.Permission} denied.");
        }

        return Task.CompletedTask;
    }
}