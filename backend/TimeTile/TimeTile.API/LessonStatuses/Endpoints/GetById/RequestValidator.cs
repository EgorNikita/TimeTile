using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.LessonStatuses.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetLessonStatusByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeValidId();
        }
    }
}
