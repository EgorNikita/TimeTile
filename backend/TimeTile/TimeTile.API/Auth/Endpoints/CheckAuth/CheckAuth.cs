using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Http;
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

    private static Task<Results<Ok<Result<Response>>, UnauthorizedHttpResult, NotFound<Result>>> Handle(
            IUserProvider userProvider, 
            CancellationToken cancellationToken
        )
    {
        var userResult = userProvider.GetUser();
        if (userResult.IsFailure)
            return Task.FromResult<Results<Ok<Result<Response>>, UnauthorizedHttpResult, NotFound<Result>>>
                (TypedResults.Unauthorized());
        
        if (userResult.Data == null)
        {
            var error = Error.From("Login failed, user data is null.");
            return Task.FromResult<Results<Ok<Result<Response>>, UnauthorizedHttpResult, NotFound<Result>>>
                (TypedResults.NotFound(Result.Failure(error)));
        }
        
        var user = userResult.Data;
        
        var response = new Response(user.Id,user.Institution!.Id);
        return Task.FromResult<Results<Ok<Result<Response>>, UnauthorizedHttpResult, NotFound<Result>>>
            (TypedResults.Ok(Result.Success(response)));
    }
    
    private record Response(int UserId, int InstitutionId);
}