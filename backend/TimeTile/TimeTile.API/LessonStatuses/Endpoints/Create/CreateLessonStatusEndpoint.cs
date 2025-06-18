using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.Create
{
    public class CreateLessonStatusEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new LessonStatus")
                .WithRequestValidation<Request>();
        }

        private static async Task<Results<Created<Result<Response>>, BadRequest<Result>, JsonHttpResult<Result>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = httpContext.GetInstitutionId();

            // Check if already exists
            var duplicateCheckResult = await IsLessonStatusDuplicate(request, institutionId, db, cancellationToken);

            if (duplicateCheckResult.IsFailure)
                return TypedResults.BadRequest(duplicateCheckResult);

            // Save LessonStatus
            var lessonStatus = new LessonStatus
            {
                Description = request.Description.Trim(),
                ArgbColor = request.ArgbColor,
                InstitutionId = institutionId
            };

            await db.LessonStatuses.AddAsync(lessonStatus, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                lessonStatus.Id,
                lessonStatus.Description,
                lessonStatus.ArgbColor
            );

            var result = Result.Success(response);

            return TypedResults.Created($"/lesson-statuses/{lessonStatus.Id}", result);
        }

        private static async Task<Result> IsLessonStatusDuplicate(
            Request request,
            int institutionId,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var isDuplicate = await db.LessonStatuses
                .AsNoTracking()
                .Where(c => c.InstitutionId == institutionId)
                .AnyAsync(c => c.Description == request.Description, cancellationToken);

            if (isDuplicate)
            {
                var error = Error.From(
                    $"A lessons status with the description '{request.Description}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            return Result.Success();
        }

        public sealed record Request(
            string Description,
            int ArgbColor
        );

        private sealed record Response(
            int Id,
            string Description,
            int ArgbColor
        );
    }
}
