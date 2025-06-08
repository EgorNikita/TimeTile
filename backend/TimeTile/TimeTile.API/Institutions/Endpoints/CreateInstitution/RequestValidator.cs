using FluentValidation;
using TimeTile.API.Common;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Institutions.Endpoints.CreateInstitution;

public class RequestValidator : AbstractValidator<CreateInstitutionEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Title contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Title);

        RuleFor(x => x.Address)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Address contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Address);

        RuleFor(x => x.PhoneNumber)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("PhoneE164 contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.PhoneE164);

        RuleFor(x => x.Email)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Email contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Email);

        RuleFor(x => x.Domain)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Domain contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Domain);
    }
}