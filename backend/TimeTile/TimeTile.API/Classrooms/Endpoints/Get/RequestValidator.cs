using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Classrooms.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetClassroomsEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetClassroomsEndpoint.Request, AllowedSortFields>();
        }
    }
}
