using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Submit
{
    public class RequestParametersValidator : AbstractValidator<SubmitSubmissionEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider, IUserProvider userProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userId = userProvider.GetUserId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<SubmitSubmissionEndpoint.RequestParameters, Submission>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Id)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    return await db.Submissions
                                        .AsNoTracking()
                                        .AnyAsync(s => s.Id == id && s.Assignment.Lesson.Course.InstitutionId == institutionId, cancellationToken);
                                })
                                .WithMessage("There is no Submission with this id in your current institution")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.Id)
                                        .MustAsync(async (id, cancellationToken) =>
                                        {
                                            return await db.Submissions
                                                .AsNoTracking()
                                                .AnyAsync(s => s.Id == id && s.StudentId == userId, cancellationToken);
                                        })
                                        .WithMessage("There is no association between you and the passed Submission")
                                        .DependentRules(() =>
                                        {
                                            RuleFor(x => x.Id)
                                                .MustAsync(async (id, cancellationToken) =>
                                                {
                                                    var submission = await db.Submissions
                                                        .AsNoTracking()
                                                        .Include(s => s.Assignment)
                                                        .FirstAsync(s => s.Id == id, cancellationToken);

                                                    if (submission.Assignment.Deadline < DateTimeOffset.UtcNow && 
                                                        !submission.Assignment.UploadAfterDeadline)
                                                    {
                                                        return false;
                                                    }

                                                    return true;
                                                })
                                                .WithMessage("Deadline is missed. You cannot submit any Submission anymore.");
                                        });
                                });
                        });
                });
        }
    }
}
