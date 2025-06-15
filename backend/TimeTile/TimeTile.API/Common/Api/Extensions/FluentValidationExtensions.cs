using FluentValidation;

namespace TimeTile.API.Common.Api.Extensions
{
    public static class FluentValidationExtensions
    {
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
