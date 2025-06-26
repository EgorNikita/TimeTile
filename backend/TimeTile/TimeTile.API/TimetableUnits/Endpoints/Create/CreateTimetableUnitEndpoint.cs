using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.TimetableUnits.Endpoints.Create
{
    public class CreateTimetableUnitEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new TimetableUnit")
                .WithRequestValidation<Request>();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

            // Save timetable unit
            var timetableUnit = new TimetableUnit
            {
                InstitutionId = institutionId,
                Title = request.Title.Trim(),
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            await db.TimetableUnits.AddAsync(timetableUnit, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                timetableUnit.Id,
                timetableUnit.Title,
                timetableUnit.StartTime,
                timetableUnit.EndTime
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{API.Endpoints.Routes.TimetableUnits}/{timetableUnit.Id}", result);
        }

        public sealed record Request(
            string Title,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime
        );

        private sealed record Response(
            int Id,
            string Title,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime
        );
    }
}
