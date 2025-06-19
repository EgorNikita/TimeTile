using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models.Interfaces;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Common.Api.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidString<T>(
            this IRuleBuilder<T, string> ruleBuilder)
            where T : class
        {
            return ruleBuilder
                .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidTitle<T>(
            this IRuleBuilder<T, string> ruleBuilder) 
            where T : class
        {
            return ruleBuilder
                .MustBeValidString()
                .WithMessage("Title contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Title);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidDescription<T>(
            this IRuleBuilder<T, string> ruleBuilder)
            where T : class
        {
            return ruleBuilder
                .MustBeValidString()
                .WithMessage("Description contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Description);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidAddress<T>(
            this IRuleBuilder<T, string> ruleBuilder)
            where T : class
        {
            return ruleBuilder
                .MustBeValidString()
                .WithMessage("Address contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Address);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidPhoneNumber<T>(
            this IRuleBuilder<T, string> ruleBuilder)
            where T : class
        {
            return ruleBuilder
                .MustBeValidString()
                .WithMessage("PhoneE164 contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.PhoneE164);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidEmail<T>(
            this IRuleBuilder<T, string> ruleBuilder)
            where T : class
        {
            return ruleBuilder
                .MustBeValidString()
                .WithMessage("Email contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Email);
        }

        public static IRuleBuilderOptions<T, string> MustBeValidName<T>(
            this IRuleBuilder<T, string> ruleBuilder)
            where T : class
        {
            return ruleBuilder
                .MustBeValidString()
                .WithMessage("Name contains invalid characters.")
                .ApplyRegexPattern(RegexPatterns.Pattern.Name);
        }

        public static IRuleBuilderOptions<T, int> MustBeValidId<T>(
            this IRuleBuilder<T, int> ruleBuilder) 
            where T : class
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id must be greater or equal to 1");
        }

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

        public static IRuleBuilderOptions<T, int> MustBeValidEntityId<T, TEntity>(
            this IRuleBuilder<T, int> ruleBuilder,
            TimetileDbContext db,
            int institutionId)
            where TEntity : class, IInstitutionEntity
        {
            return ruleBuilder.MustAsync(async (id, cancellationToken) =>
            {
                return await db.Set<TEntity>()
                    .Where(e => e.InstitutionId == institutionId)
                    .AnyAsync(e => e.Id == id, cancellationToken);
            }).WithMessage($"{typeof(TEntity).Name}'s ID is invalid.");
        }

        public static IRuleBuilderOptions<T, string?> MustBeValidSortField<T, TEnum>(this IRuleBuilder<T, string?> ruleBuilder)
            where TEnum : Enum
        {
            var allowedSortFields = Enum
                .GetNames(typeof(TEnum))
                .Select(name => name.ToLower())
                .ToArray();

            return ruleBuilder
                .Must(sortBy => string.IsNullOrEmpty(sortBy) || allowedSortFields.Contains(sortBy.ToLower()))
                .WithMessage($"SortBy must be one of the following: {string.Join(", ", allowedSortFields)}");
        }
    }
}
