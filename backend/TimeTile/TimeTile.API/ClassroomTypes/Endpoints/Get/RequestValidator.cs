using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.ClassroomTypes.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetClassroomTypesEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetClassroomTypesEndpoint.Request, AllowedSortFields>();
        }
    }
}
