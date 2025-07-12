using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Update
{
    public class RequestParametersValidator : AbstractValidator<UpdateSubmissionEndpoint.RequestParameters>
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
                        .MustBeValidEntityId<UpdateSubmissionEndpoint.RequestParameters, Submission>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Id)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    var submission = await db.Submissions
                                        .Include(s => s.Student)
                                        .FirstAsync(a => a.Id == id, cancellationToken);

                                    return submission.Student.InstitutionId == institutionId;
                                })
                                .WithMessage("There is no Submission with this id in your current institution")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.Id)
                                        .MustAsync(async (id, cancellationToken) =>
                                        {
                                            var isRequestFromStudent = await db.Submissions
                                                .AsNoTracking()
                                                .AnyAsync(s => s.Id == id && s.StudentId == userId, cancellationToken);

                                            if (!isRequestFromStudent)
                                            {
                                                return await db.Submissions
                                                    .AsNoTracking()
                                                    .AnyAsync(s => s.Id == id && s.Assignment.Lesson.Course.TeacherId == userId, cancellationToken);
                                            }

                                            return true;
                                        })
                                        .WithMessage("There is no association between you and the passed Submission");
                                });
                        });
                });
        }
    }
}
