using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Authentication.Endpoints.LogoutAll;

// Additional endpoint for logging out from all devices
public class LogoutAll : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/logout-all", Handle)
            .WithSummary("Revokes all refresh tokens for current user")
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
        IAuthService authService,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var ipAddress = userProvider.GetUserIp();
        
        await authService.RevokeAllUserTokens(userId, ipAddress, cancellationToken);

        Log.Information("User {UserId} logged out from ALL devices from IP {IpAddress}", userId, ipAddress);

        var response = Result.Success(new Response("Successfully logged out from all devices"));
        return TypedResults.Ok(response);
    }

    private record Response(string Message);
}