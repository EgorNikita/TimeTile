using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateAssignmentEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Title)
                .MustBeValidTitle();

            RuleFor(x => x.Description)
                .MustBeValidDescription();

            RuleFor(x => x.Deadline)
                .Must(deadline => deadline > DateTimeOffset.UtcNow);

            RuleFor(x => x.LessonId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.LessonId)
                        .MustBeValidEntityId<CreateAssignmentEndpoint.Request, Lesson>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.LessonId)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    var lesson = await db.Lessons
                                        .Include(l => l.Course)
                                        .FirstAsync(l => l.Id == id, cancellationToken);

                                    return lesson.Course.InstitutionId == institutionId;
                                })
                                .WithMessage("There is no Lesson with this id in your current institution")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.LessonId)
                                        .MustAsync(async (id, cancellationToken) =>
                                        {
                                            var lesson = await db.Lessons
                                                .FirstAsync(l => l.Id == id, cancellationToken);

                                            return lesson.AssignmentId == null;
                                        })
                                        .WithMessage("Lesson is already associated with some Assignment");
                                });
                        });
                });
        }
    }
}
