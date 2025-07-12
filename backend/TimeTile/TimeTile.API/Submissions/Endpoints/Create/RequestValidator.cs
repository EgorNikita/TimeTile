using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateSubmissionEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider, IUserProvider userProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userId = userProvider.GetUserId();

            RuleFor(x => x.AssignmentId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.AssignmentId)
                        .MustBeValidEntityId<CreateSubmissionEndpoint.Request, Assignment>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.AssignmentId)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    return await db.Assignments
                                        .AsNoTracking()
                                        .AnyAsync(a => a.Id == id && a.Lesson.Course.InstitutionId == institutionId, cancellationToken);
                                })
                                .WithMessage("There is no Assignment with this id in your current institution")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x)
                                        .MustAsync(async (request, cancellationToken) =>
                                        {
                                            return await db.Lessons
                                                .AnyAsync(l =>
                                                    l.AssignmentId != null &&
                                                    l.AssignmentId == request.AssignmentId &&
                                                    l.Course.CoursesToStudents.Any(cs => cs.StudentId == userId), cancellationToken
                                                );
                                        })
                                        .WithMessage("There is no association between you and the passed Assignment")
                                        .DependentRules(() =>
                                        {
                                            RuleFor(x => x)
                                                .MustAsync(async (request, cancellationToken) =>
                                                {
                                                    return !await db.Submissions
                                                        .AnyAsync(s =>
                                                            s.AssignmentId == request.AssignmentId &&
                                                            s.StudentId == userId &&
                                                            s.Status != Core.Enums.SubmissionStatus.Rejected, cancellationToken
                                                        );
                                                })
                                                .WithMessage("There is already one Submission with such data.");
                                        }); ;
                                });
                        });
                });

            RuleFor(x => x)
                .Must(x => (x.GradeValue != null && x.GradeWeight != null) || x.GradeValue == x.GradeWeight)
                .WithMessage("Grade is not passed properly.");

            RuleFor(x => x)
                .Must(x => (x.GradeValue == null && x.GradeWeight == null) || x.Status == Core.Enums.SubmissionStatus.Accepted)
                .WithMessage("Grade can be passed only if Submission status is Accepted.");


            When(x => x.GradeValue != null, () =>
            {
                RuleFor(x => (int)x.GradeValue!.Value)
                    .GreaterThan(0)
                    .WithMessage("Value of grade should be greater than zero");
            });

            When(x => x.GradeWeight != null, () =>
            {
                RuleFor(x => x.GradeWeight!.Value)
                    .GreaterThan(0)
                    .WithMessage("Weight should be greater than zero");
            });
        }
    }
}