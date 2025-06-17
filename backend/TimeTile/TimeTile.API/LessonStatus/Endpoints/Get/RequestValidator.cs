using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.LessonStatus.Endpoints.Get
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
