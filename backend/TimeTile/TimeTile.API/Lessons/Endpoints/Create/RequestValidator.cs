using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateLessonEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.TimetableUnitId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.TimetableUnitId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, TimetableUnit>(db, institutionId);
                });

            RuleFor(x => x.CourseId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.CourseId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, Course>(db, institutionId);
                });

            RuleFor(x => x.ClassroomId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.ClassroomId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, Classroom>(db, institutionId);
                });

            RuleFor(x => x.LessonStatusId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.LessonStatusId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, LessonStatus>(db, institutionId);
                });

            RuleFor(x => x.Date)
                .Must(date => date >= DateTimeOffset.UtcNow)
                .WithMessage("Date should be from future");

            RuleFor(x => x.Description)
                .MustBeValidDescription();

            RuleFor(x => x.HomeworkDescription)
                .MustBeValidDescription();
        }
    }
}
