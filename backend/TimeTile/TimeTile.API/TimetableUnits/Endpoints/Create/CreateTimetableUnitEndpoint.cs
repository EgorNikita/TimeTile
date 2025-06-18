using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
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

        private static async Task<Results<Created<Result<Response>>, BadRequest<Result>, JsonHttpResult<Result>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = httpContext.GetInstitutionId();

            // Check if already exists
            var duplicateCheckResult = await IsTimetableUnitDuplicate(request, institutionId, db, cancellationToken);

            if (duplicateCheckResult.IsFailure)
                return TypedResults.BadRequest(duplicateCheckResult);

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

            return TypedResults.Created($"/timetable-units/{timetableUnit.Id}", result);
        }

        private static async Task<Result> IsTimetableUnitDuplicate(
            Request request,
            int institutionId,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var baseQuery = db.TimetableUnits
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId && x.DeletedAt == null);

            // Check timetable_units_institution_title_deleted_at_key
            var isDuplicateOnTitle = await baseQuery
                .AnyAsync(x => x.Title == request.Title, cancellationToken);

            if (isDuplicateOnTitle)
            {
                var error = Error.From(
                    $"A timetable unit with the title '{request.Title}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            // Check timetable_units_institution_start_end_deleted_at_key
            var allUnits = await baseQuery
                .ToListAsync(cancellationToken);

            var requestStartTimeUtc = request.StartTime.UtcDateTime.TimeOfDay;
            var requestEndTimeUtc = request.EndTime.UtcDateTime.TimeOfDay;

            var isDuplicateOnTime = allUnits.
                Any(x => x.StartTime.UtcDateTime.TimeOfDay == requestStartTimeUtc &&
                    x.EndTime.UtcDateTime.TimeOfDay == requestEndTimeUtc);

            if (isDuplicateOnTime)
            {
                var error = Error.From(
                    $"A timetable unit with the time '{requestStartTimeUtc}' - '{requestEndTimeUtc}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            return Result.Success();
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
