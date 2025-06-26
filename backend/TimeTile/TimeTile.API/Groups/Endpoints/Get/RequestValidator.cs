using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Groups.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetGroupsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetGroupsEndpoint.Request, AllowedSortFields>();

            // InstitutionMemberIds
            RuleFor(x => x.InstitutionMemberIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.InstitutionMemberIds != null, () => {
                        RuleFor(x => x.InstitutionMemberIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetGroupsEndpoint.Request, InstitutionMember>(db, institutionId);
                    });
                });

            // CourseIds
            RuleFor(x => x.CourseIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.CourseIds != null, () => {
                        RuleFor(x => x.CourseIds!)
                            .MustBeValidInstitutionEntityIdsList<GetGroupsEndpoint.Request, Course>(db, institutionId);
                    });
                });
        }
    }
}
