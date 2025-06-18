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
                .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
                .WithMessage("Description contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Description);
        }
    }
}
