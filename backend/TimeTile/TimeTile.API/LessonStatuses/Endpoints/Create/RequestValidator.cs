using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.LessonStatuses.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateLessonStatusEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Description)
                .MustBeValidDescription();
        }
    }
}
