using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.GetStudents
{
    public class RequestValidator : PagedRequestValidator<GetCourseStudentsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetCourseStudentsEndpoint.Request, AllowedSortFields>();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidInstitutionEntityId<GetCourseStudentsEndpoint.Request, Course>(db, institutionId);
                });
        }
    }
}
