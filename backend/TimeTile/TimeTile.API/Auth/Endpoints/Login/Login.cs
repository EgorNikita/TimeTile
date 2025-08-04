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
            .WithSummary("Logs in user and returns user data")
            .WithRequestValidation<Request>();
    }

    private static async Task<Ok<Result<Response>>>
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
        
        if (result.IsFailure || result.Data == null)
        {
            var error = Error.From("Unauthorized", "UNAUTHORIZED");
            return TypedResults.Ok(Result.Failure<Response>(error));
        }
        
        var response = new Response(
            result.Data.Id,
            result.Data.Institution!.Id);
        return TypedResults.Ok(Result.Success(response));
    }
    
    public record Request(string Login, string Password, bool RememberMe);

    private record Response(int UserId, int InstitutionId);
}