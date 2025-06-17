using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Terms.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateTermEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
                .WithMessage("Title contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Title);

            RuleFor(x => x.EndDate.UtcDateTime.Date)
                .GreaterThan(x => x.StartDate.UtcDateTime.Date)
                .WithMessage("EndDate must be after StartDate.");
        }
    }
}
