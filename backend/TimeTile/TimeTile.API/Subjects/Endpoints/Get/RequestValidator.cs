using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Subjects.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetSubjectsEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetSubjectsEndpoint.Request, AllowedSortFields>();
        }
    }
}
