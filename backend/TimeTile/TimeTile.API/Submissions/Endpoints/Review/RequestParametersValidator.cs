using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Review
{
    public class RequestParametersValidator : AbstractValidator<ReviewSubmissionEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider, IUserProvider userProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userResult = userProvider.GetUser();
            if (!userResult.IsSuccess || userResult.Data == null)
                throw new UnauthorizedAccessException("Current user is not available.");
            var userId = userResult.Data.Id;

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<ReviewSubmissionEndpoint.RequestParameters, Submission>(db)
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
                                                .AnyAsync(s => s.Id == id && s.Assignment.Lesson.Course.TeacherId == userId, cancellationToken);
                                        })
                                        .WithMessage("There is no association between you and the passed Submission");
                                });
                        });
                });
        }
    }
}
