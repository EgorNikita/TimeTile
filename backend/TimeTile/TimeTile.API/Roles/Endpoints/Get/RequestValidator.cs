using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Roles.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetRolesEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetRolesEndpoint.Request, AllowedSortFields>();
        }
    }
}
