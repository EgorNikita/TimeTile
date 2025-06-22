using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.InstitutionMembers.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetInstitutionMembersEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetInstitutionMembersEndpoint.Request, AllowedSortFields>();
        }
    }
}
