using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Courses.Endpoints.Get;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetCourses
{
    public class RequestValidator : PagedRequestValidator<GetStudentCoursesEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetStudentCoursesEndpoint.Request, AllowedSortFields>();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidOptionalInstitutionEntityId<GetStudentCoursesEndpoint.Request, Student>(db, institutionId);
                });

            // TermIds
            RuleFor(x => x.TermIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.TermIds != null, () => {
                        RuleFor(x => x.TermIds!)
                            .MustBeValidInstitutionEntityIdsList<GetStudentCoursesEndpoint.Request, Term>(db, institutionId);
                    });
                });
        }
    }
}
