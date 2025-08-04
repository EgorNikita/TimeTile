using Microsoft.AspNetCore.Authorization;
using TimeTile.Core.Common.Constants;

namespace TimeTile.API.Auth.Authorization;

public static class PermissionHandlerAuthorizationExtension
{
    public static void AddPermissionHandlerAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
                options.AddPolicy(permission,
                    policy => { policy.Requirements.Add(new PermissionRequirement(permission)); });
        });

        builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
    }
}