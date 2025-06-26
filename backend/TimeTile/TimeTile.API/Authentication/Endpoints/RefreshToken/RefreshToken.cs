using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Authentication.Endpoints.RefreshToken;

public class RefreshToken : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/refresh", Handle)
            .WithSummary("Refreshes access token using refresh token")
            .WithRequestValidation<Request>();
    }

    private static async Task<
        Results<
            Ok<Result<Response>>, 
            UnauthorizedHttpResult, 
            BadRequest<Result>>> 
        Handle(
        Request request,
        IAuthService authService,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var ipAddress = userProvider.GetUserIp();
        var result = await authService.RefreshToken(request.RefreshToken, ipAddress, cancellationToken);

        if (!result.Success)
            return TypedResults.Unauthorized();

        if (result.AccessToken == null || result.RefreshToken == null)
        {
            var error = Error.From(
                "Failed to generate tokens", 
                "TOKEN_GENERATION_FAILED"
            );

            return TypedResults.BadRequest(Result.Failure(error));
        }
        
        var response = Result.Success(new Response(result.AccessToken, result.RefreshToken));
        return TypedResults.Ok(response);
    }
    
    public record Request(string RefreshToken);
    private record Response(string AccessToken, string RefreshToken);
}