using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Grades.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetGradesEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetGradesEndpoint.Request, AllowedSortFields>();

            // Types
            RuleFor(x => x.Types)
                .Must(types => types == null || types.All(t => 
                    Enum.TryParse<GradeType>(t, ignoreCase: true, out _)))
                .WithMessage("One or more GradeType values are invalid.");

            // LessonIds
            RuleFor(x => x.LessonIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.LessonIds != null, () => {
                        RuleFor(x => x.LessonIds!)
                            .MustBeValidEntityIdsList<GetGradesEndpoint.Request, Lesson>(db);
                    });
                });

            // StudentIds
            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () => {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetGradesEndpoint.Request, Student>(db, institutionId);
                    });
                });

            // CourseIds
            RuleFor(x => x.CourseIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.CourseIds != null, () => {
                        RuleFor(x => x.CourseIds!)
                            .MustBeValidInstitutionEntityIdsList<GetGradesEndpoint.Request, Course>(db, institutionId);
                    });
                });
        }
    }
}
