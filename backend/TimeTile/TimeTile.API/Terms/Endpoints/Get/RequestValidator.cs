using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Students.Endpoints.GetCourses;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Terms.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetTermsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetTermsEndpoint.Request, AllowedSortFields>();

            // StudentIds
            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () => {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetTermsEndpoint.Request, Student>(db, institutionId);
                    });
                });
        }
    }
}
