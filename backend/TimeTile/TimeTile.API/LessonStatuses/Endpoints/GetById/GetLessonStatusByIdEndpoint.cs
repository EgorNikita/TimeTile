using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.GetById
{
    public class GetLessonStatusByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns LessonStatus by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Results<Ok<Result<Response>>, NotFound<Result>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extracts institutionId
            var institutionId = httpContext.GetInstitutionId();

            // Find LessonStatus
            var lessonStatus = await db.LessonStatuses
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            // Return error in case of invalid id
            if (lessonStatus is null)
            {
                var error = Error.From(
                    $"LessonStatus with id '{request.Id}' does not exist.",
                    "ENTITY_DOES_NOT_EXIST"
                );

                return TypedResults.NotFound(Result.Failure(error));
            }

            var response = new Response(
                lessonStatus.Id,
                lessonStatus.Description,
                lessonStatus.ArgbColor
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            int Id,
            string Description,
            int ArgbColor
        );
    }
}
