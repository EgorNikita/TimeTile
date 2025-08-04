using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;

namespace TimeTile.API.Auth.Endpoints.CheckAuth;

public class CheckAuth : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/check", Handle);
    }

    private static async Task<Ok<Result<Response>>> Handle(
        ITokenHandlerService tokenHandlerService,
        CancellationToken cancellationToken
        )
    {
        var userResult = await tokenHandlerService.GetCurrentUser();
        if (userResult.IsFailure || userResult.Data == null)
        {
            var error = Error.From("Unauthorized", "UNAUTHORIZED");
            return TypedResults.Ok(Result.Failure<Response>(error));

        }

        var user = userResult.Data;
        var response = new Response(user.Id, user.Institution!.Id);

        return TypedResults.Ok(Result.Success(response));
    }
    
    private record Response(int UserId, int InstitutionId);
}