using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Lessons.Endpoints.Create;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.Update
{
    public class RequestBodyValidator : AbstractValidator<UpdateLessonEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            When(x => x.TimetableUnitId != null, () =>
            {
                RuleFor(x => x.TimetableUnitId!.Value)
                    .MustBeValidId()
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.TimetableUnitId!.Value)
                            .MustBeValidInstitutionEntityId<UpdateLessonEndpoint.RequestBody, TimetableUnit>(db, institutionId);
                    });
            });

            When(x => x.CourseId != null, () =>
            {
                RuleFor(x => x.CourseId!.Value)
                   .MustBeValidId()
                   .DependentRules(() =>
                   {
                       RuleFor(x => x.CourseId!.Value)
                           .MustBeValidInstitutionEntityId<UpdateLessonEndpoint.RequestBody, Course>(db, institutionId);
                   });
            });

            When(x => x.ClassroomId != null, () =>
            {
                RuleFor(x => x.ClassroomId!.Value)
                    .MustBeValidId()
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.ClassroomId!.Value)
                            .MustBeValidInstitutionEntityId<UpdateLessonEndpoint.RequestBody, Classroom>(db, institutionId);
                    });
            });

            When(x => x.LessonStatusId != null, () =>
            {
                RuleFor(x => x.LessonStatusId!.Value)
                    .MustBeValidId()
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.LessonStatusId!.Value)
                            .MustBeValidInstitutionEntityId<UpdateLessonEndpoint.RequestBody, LessonStatus>(db, institutionId);
                    });
            });


            RuleFor(x => x.Date)
                .Must(date => date == null || date >= DateTimeOffset.UtcNow)
                .WithMessage("Date should be from future");

            When(x => x.Description != null, () =>
            {
                RuleFor(x => x.Description!)
                    .MustBeValidDescription();
            });

            When(x => x.HomeworkDescription != null, () =>
            {
                RuleFor(x => x.HomeworkDescription!)
                    .MustBeValidDescription();
            });
        }
    }
}
