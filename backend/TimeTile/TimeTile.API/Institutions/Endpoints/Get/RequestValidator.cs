using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Institutions.Endpoints.Get;

public class RequestValidator : PagedRequestValidator<GetInstitutionsEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetInstitutionsEndpoint.Request, AllowedSortFields>();
    }
}