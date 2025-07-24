using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetSubmissionsBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidEntityIdsList<GetSubmissionsBulkEndpoint.Request, Submission>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Ids)
                                .MustAsync(async (ids, cancellationToken) =>
                                {
                                    var invalidSubmissionExists = await db.Submissions
                                        .AsNoTracking()
                                        .Where(s => ids.Contains(s.Id))
                                        .AnyAsync(s => s.Assignment.Lesson.Course.InstitutionId != institutionId, cancellationToken);

                                    return !invalidSubmissionExists;
                                })
                                .WithMessage("Some of Submissions do not belong to the current institution");
                        });
                });
        }
    }
}
