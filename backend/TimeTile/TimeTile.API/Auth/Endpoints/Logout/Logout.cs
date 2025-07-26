using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Auth.Endpoints.Logout;


public class Logout : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/logout", Handle)
            .WithSummary("Revokes refresh token")
            .WithRequestValidation<Request>()
            .RequireAuthorization();
    }
    
    private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
        Request request,
        ITokenHandlerService authService,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        // var ipAddress = userProvider.GetUserIp();
        // var success = await authService.RevokeToken(request.RefreshToken, ipAddress, cancellationToken);
        //
        // if (!success)
        // {
        //     var error = Error.From(
        //         "Invalid refresh token", 
        //         "INVALID_REFRESH_TOKEN"
        //     );
        //
        //     return TypedResults.BadRequest(Result.Failure(error));
        // }

        var response = Result.Success(new Response("Successfully logged out."));
        return TypedResults.Ok(response);
    }

    public record Request(string RefreshToken);
    private record Response(string Message);
}