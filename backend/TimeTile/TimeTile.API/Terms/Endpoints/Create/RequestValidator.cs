using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Terms.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateTermEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle()
                .MustAsync(async (title, cancellationToken) =>
                {
                    title = title.Trim();

                    return !await db.Terms
                        .AsNoTracking()
                        .Where(t => t.InstitutionId == institutionId)
                        .AnyAsync(t => t.Title == title, cancellationToken);
                })
                .WithMessage("Title is already taken.");

            RuleFor(x => x.EndDate.UtcDateTime.Date)
                .GreaterThan(x => x.StartDate.UtcDateTime.Date)
                .WithMessage("EndDate must be after StartDate.");

            RuleFor(t => t)
                .MustAsync(async (request, cancellationToken) =>
                {
                    var allTerms = await db.Terms
                        .AsNoTracking()
                        .Where(t => t.InstitutionId == institutionId)
                        .ToListAsync(cancellationToken);

                    var requestStartDateUtc = request.StartDate.UtcDateTime.Date;
                    var requestEndDateUtc = request.EndDate.UtcDateTime.Date;

                    return !allTerms
                        .Any(t =>
                            t.StartDate.UtcDateTime.Date == requestStartDateUtc &&
                            t.EndDate.UtcDateTime.Date == requestEndDateUtc);
                })
                .WithMessage("Period is already taken.");
        }
    }
}
