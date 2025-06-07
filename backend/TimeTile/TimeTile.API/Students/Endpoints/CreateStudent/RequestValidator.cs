using FluentValidation;
using TimeTile.API.Common;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Students.Endpoints.CreateStudent;

public class RequestValidator : AbstractValidator<CreateStudentEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(u => u.Firstname)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Firstname contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Name);
        
        RuleFor(u => u.Lastname)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Lastname contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Name);
        
        RuleFor(u => u.BirthDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("BirthDate cannot be in the future.");


        RuleFor(u => u.PhoneNumber)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("PhoneNumber contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.PhoneE164);
    }
}