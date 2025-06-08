using FluentValidation;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Common;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ApplyRegexPattern<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        RegexPatterns.Pattern patternKey,
        bool allowEmpty = false) where T : class
    {
        var patternInfo = RegexPatterns.Patterns[patternKey];

        var options = allowEmpty
            ? ruleBuilder
                .Matches(patternInfo.Pattern).WithMessage(patternInfo.Description)
                .MaximumLength(patternInfo.MaxLength)
                .WithMessage($"{patternKey} cannot exceed {patternInfo.MaxLength} characters.")
            : ruleBuilder
                .NotEmpty().WithMessage($"{patternKey} is required.")
                .Matches(patternInfo.Pattern).WithMessage(patternInfo.Description)
                .MaximumLength(patternInfo.MaxLength)
                .WithMessage($"{patternKey} cannot exceed {patternInfo.MaxLength} characters.");

        return options;
    }
}