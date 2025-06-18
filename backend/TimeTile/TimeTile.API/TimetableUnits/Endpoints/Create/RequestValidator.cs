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

            RuleFor(x => x.EndTime.UtcDateTime.TimeOfDay)
                .GreaterThan(x => x.StartTime.UtcDateTime.TimeOfDay)
                .WithMessage("EndTime must be after StartTime.");
        }
    }
}
