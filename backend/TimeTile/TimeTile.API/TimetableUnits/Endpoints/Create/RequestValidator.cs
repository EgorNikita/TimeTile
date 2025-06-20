using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.TimetableUnits.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateTimetableUnitEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle()
                .MustAsync(async (title, cancellationToken) =>
                {
                    title = title.Trim();

                    return !await db.TimetableUnits
                        .AsNoTracking()
                        .Where(x => x.InstitutionId == institutionId)
                        .AnyAsync(x => x.Title == title, cancellationToken);
                })
                .WithMessage("Title is already taken.");

            RuleFor(x => x.EndTime.UtcDateTime.TimeOfDay)
                .GreaterThan(x => x.StartTime.UtcDateTime.TimeOfDay)
                .WithMessage("EndTime must be after StartTime.");

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                {
                    var allUnits = await db.TimetableUnits
                        .AsNoTracking()
                        .Where(x => x.InstitutionId == institutionId)
                        .ToListAsync(cancellationToken);

                    var requestStartTimeUtc = request.StartTime.UtcDateTime.TimeOfDay;
                    var requestEndTimeUtc = request.EndTime.UtcDateTime.TimeOfDay;

                    return !allUnits
                        .Any(x => 
                            x.StartTime.UtcDateTime.TimeOfDay == requestStartTimeUtc &&
                            x.EndTime.UtcDateTime.TimeOfDay == requestEndTimeUtc);
                })
                .WithMessage("Period is already taken.");
        }
    }
}
