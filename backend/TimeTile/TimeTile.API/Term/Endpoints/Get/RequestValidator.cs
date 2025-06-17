using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Term.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetTermsEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetTermsEndpoint.Request, AllowedSortFields>();
        }
    }
}
