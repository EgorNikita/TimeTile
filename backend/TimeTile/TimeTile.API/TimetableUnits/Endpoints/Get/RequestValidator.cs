using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.TimetableUnits.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetTimetableUnitsEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetTimetableUnitsEndpoint.Request, AllowedSortFields>();
        }
    }
}
