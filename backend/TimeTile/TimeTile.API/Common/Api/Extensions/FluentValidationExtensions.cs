using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Common.Regex;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Common.Api.Extensions
{
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

        public static IRuleBuilderOptions<T, int> MustBeValidForeignKey<T, TEntity>(
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
