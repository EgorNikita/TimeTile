using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Auth.Endpoints.Logout;

public class Logout : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/logout", Handle);
    }
    
    private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
        ITokenHandlerService tokenHandlerService,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        await tokenHandlerService.Logout();

        var response = Result.Success(new Response("Successfully logged out."));
        return TypedResults.Ok(response);
    }
    
    private record Response(string Message);
}