using TimeTile.API.Authentication;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Common.Api.Filters
{
    public class RequireInstitutionFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;
            var claimsPrincipal = httpContext.User;
            var db = httpContext.RequestServices.GetRequiredService<TimetileDbContext>();
            var cancellationToken = httpContext.RequestAborted;

            var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);

            if (institutionResult.IsFailure)
            {
                return TypedResults.Json(
                    Result.Failure(institutionResult.Error),
                    statusCode: StatusCodes.Status401Unauthorized
                );
            }

            // Store institution ID for use in the endpoint
            httpContext.Items[HttpContextItemKeys.InstitutionId] = institutionResult.Data;

            return await next(context);
        }
    }
}
