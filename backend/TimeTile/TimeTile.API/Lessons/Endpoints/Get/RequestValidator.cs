using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetLessonsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            // StudentIds
            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () => {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetLessonsEndpoint.Request, Student>(db, institutionId);
                    });
                });

            // GroupIds
            RuleFor(x => x.GroupIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.GroupIds != null, () => {
                        RuleFor(x => x.GroupIds!)
                            .MustBeValidInstitutionEntityIdsList<GetLessonsEndpoint.Request, Group>(db, institutionId);
                    });
                });

            // ClassroomIds
            RuleFor(x => x.ClassroomIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.ClassroomIds != null, () => {
                        RuleFor(x => x.ClassroomIds!)
                            .MustBeValidInstitutionEntityIdsList<GetLessonsEndpoint.Request, Classroom>(db, institutionId);
                    });
                });

            // TimetableUnitIds
            RuleFor(x => x.TimetableUnitIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.TimetableUnitIds != null, () => {
                        RuleFor(x => x.TimetableUnitIds!)
                            .MustBeValidInstitutionEntityIdsList<GetLessonsEndpoint.Request, TimetableUnit>(db, institutionId);
                    });
                });

            // CourseIds
            RuleFor(x => x.CourseIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.CourseIds != null, () => {
                        RuleFor(x => x.CourseIds!)
                            .MustBeValidInstitutionEntityIdsList<GetLessonsEndpoint.Request, Course>(db, institutionId);
                    });
                });

            // LessonStatusIds
            RuleFor(x => x.LessonStatusIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.LessonStatusIds != null, () => {
                        RuleFor(x => x.LessonStatusIds!)
                            .MustBeValidEntityIdsList<GetLessonsEndpoint.Request, LessonStatus>(db);
                    });
                });

            // Date
            RuleFor(x => x)
                .Must(x => !x.From.HasValue || !x.Until.HasValue || x.From <= x.Until)
                .WithMessage("From must be less than or equal to Until");
        }
    }
}
