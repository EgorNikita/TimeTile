using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.LessonStatuses.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetLessonStatusesEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetLessonStatusesEndpoint.Request, AllowedSortFields>();
        }
    }
}
