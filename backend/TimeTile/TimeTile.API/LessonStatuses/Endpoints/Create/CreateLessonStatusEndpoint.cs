using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
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

        private static async Task<Created<Result<Response>>> Handle(
            Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

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

            return TypedResults.Created($"/{API.Endpoints.Routes.LessonStatuses}/{lessonStatus.Id}", result);
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
