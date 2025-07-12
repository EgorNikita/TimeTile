using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetSubmissionByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<GetSubmissionByIdEndpoint.Request, Submission>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Id)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    return await db.Submissions
                                        .AsNoTracking()
                                        .AnyAsync(s => s.Id == id && s.Assignment.Lesson.Course.InstitutionId == institutionId, cancellationToken);
                                })
                                .WithMessage("There is no Submission with this id in your current institution");
                        });
                });
        }
    }
}
