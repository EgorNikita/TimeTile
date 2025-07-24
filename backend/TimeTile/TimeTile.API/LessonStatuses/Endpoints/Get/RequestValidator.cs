using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.LessonStatuses.Endpoints.Get
{
    public class RequestValidator : AbstractValidator<GetLessonStatusesEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetLessonStatusesEndpoint.Request, AllowedSortFields>();
        }
    }
}
