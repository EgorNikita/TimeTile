using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Authentication.Endpoints.Login;

public class Login : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/login", Handle)
            .WithSummary("Authenticates a user and returns a JWT token with roles and institution claims")
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
        var loginResult = await authService.Login(request.Login, request.Password, ipAddress, cancellationToken);
       
        if (!loginResult.Success)
            return TypedResults.Unauthorized();

        if (loginResult.AccessToken == null || loginResult.RefreshToken == null)
        {
            var error = Error.From(
                "Failed to generate tokens", 
                "TOKEN_GENERATION_FAILED"
            );

            return TypedResults.BadRequest(Result.Failure(error));
        }
        
        var response  = Result.Success(
            new Response(
                loginResult.AccessToken, 
                loginResult.RefreshToken
            ));
        
        return TypedResults.Ok(response);
    }
    
    public record Request(string Login, string Password);

    private record Response(string AccessToken, string RefreshToken);
}