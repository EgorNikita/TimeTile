using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Auth.Endpoints.Login;

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
            NotFound<Result>>>
        Handle(
        Request request,
        ITokenHandlerService tokenHandlerService,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var result = await tokenHandlerService.Login(
            request.Login, 
            request.Password,
            request.RememberMe,
            httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString());
        
        if (result.IsFailure)
        {
            return TypedResults.Unauthorized();
        }
        
        if (result.Data == null)
        {
           var error = Error.From("Login failed, user data is null.");
            return TypedResults.NotFound(Result.Failure(error));
        }
        
        var response = new Response(
            result.Data.Id,
            result.Data.Institution!.Id);
        return TypedResults.Ok(Result.Success(response));
    }
    
    public record Request(string Login, string Password, bool RememberMe);

    private record Response(int UserId, int InstitutionId);
}