using Microsoft.AspNetCore.Authorization;

namespace TimeTile.API.Common.Api;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        Serilog.Log.Information($"Checking permission: {requirement.Permission}");
        var hasPermission = context.User.HasClaim(c => c.Type == "permission" && c.Value == requirement.Permission);
        
        if (hasPermission)
        {
            Serilog.Log.Information($"Permission {requirement.Permission} granted.");
            context.Succeed(requirement);
        }
        else
            Serilog.Log.Information($"Permission {requirement.Permission} denied.");
        
        return Task.CompletedTask;
    }
}