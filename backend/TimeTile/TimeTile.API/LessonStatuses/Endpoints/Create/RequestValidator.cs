using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Regex;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateLessonStatusEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Description)
                .MustBeValidDescription()
                .MustAsync(async (description, cancellationToken) =>
                {
                    description = description.Trim();

                    return !await db.LessonStatuses
                        .Where(x => x.InstitutionId == institutionId)
                        .AnyAsync(x => x.Description == description, cancellationToken);
                })
                .WithMessage("Description is already taken.");
        }
    }
}
