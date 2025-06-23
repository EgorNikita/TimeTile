using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Subjects.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateSubjectEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Title)
                        .MustAsync(async (title, cancellationToken) =>
                        {
                            title = title.Trim();

                            return !await db.Subjects
                                .AsNoTracking()
                                .Where(x => x.InstitutionId == institutionId)
                                .AnyAsync(x => x.Title == title, cancellationToken);
                        })
                        .WithMessage("Title is already taken.");
                });
        }
    }
}
