using TimeTile.API.Authentication;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Common.Api.Filters;

public class RequireUserIdFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        
        if (!httpContext.User.Identity?.IsAuthenticated == true)
        {
            return Results.Unauthorized();
        }
        
        var claimsPrincipal = httpContext.User;
        var db = httpContext.RequestServices.GetRequiredService<TimetileDbContext>();
        var cancellationToken = httpContext.RequestAborted;

        var userResult = await claimsPrincipal.GetValidatedUserIdAsync(db, cancellationToken);

        if (userResult.IsFailure)
        {
            return TypedResults.Json(
                Result.Failure(userResult.Error),
                statusCode: StatusCodes.Status403Forbidden
            );
        }

        // Store user ID for use in the endpoint
        httpContext.Items[HttpContextItemKeys.UserId] = userResult.Data;

        return await next(context);
    }
}