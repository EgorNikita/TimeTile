using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.TimetableUnits.Endpoints.GetById
{
    public class GetTimetableUnitByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns TimetableUnit by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Results<Ok<Result<Response>>, NotFound<Result>, JsonHttpResult<Result>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var institutionId = httpContext.GetInstitutionId();

            // Find TimetableUnit
            var timetableUnit = await db.TimetableUnits
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            // Return error in case of invalid id
            if (timetableUnit is null)
            {
                var error = Error.From(
                    $"TimetableUnit with id '{request.Id}' does not exist.",
                    "ENTITY_DOES_NOT_EXIST"
                );

                return TypedResults.NotFound(Result.Failure(error));
            }

            var response = new Response(
                timetableUnit.Id,
                timetableUnit.Title,
                timetableUnit.StartTime,
                timetableUnit.EndTime
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            int Id,
            string Title,
            DateTimeOffset StartTime,
            DateTimeOffset EndTime
        );
    }
}
