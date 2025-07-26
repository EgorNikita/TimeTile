using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Common.Interfaces.Services;

namespace TimeTile.API.Common.Api.Filters;

public class UserContextFilter : IEndpointFilter
{
    private readonly ITokenHandlerService _tokenHandlerService;

    public UserContextFilter(ITokenHandlerService tokenHandlerService)
    {
        _tokenHandlerService = tokenHandlerService;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Get current user from secure cookie
        var userResult = await _tokenHandlerService.GetCurrentUser();
        
        if (userResult.IsFailure || userResult.Data == null)
        {
            return TypedResults.Json(
                Result.Failure(userResult.Error),
                statusCode: StatusCodes.Status401Unauthorized
            );
        }
        
        context.HttpContext.Items["CurrentUser"] = userResult.Data;

        return await next(context);
    }
}