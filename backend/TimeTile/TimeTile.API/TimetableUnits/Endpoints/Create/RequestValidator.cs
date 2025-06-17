using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.TimetableUnits.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateTimetableUnitEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
                .WithMessage("Title contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Title);

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("EndTime cannot be before StartTime.");
        }
    }
}
