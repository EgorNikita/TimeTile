using FluentValidation;

namespace TimeTile.API.Students.Endpoints.GetStudents;

public class RequestValidator : AbstractValidator<GetStudentsEndpoint.Request>
{
    private static readonly string[] AllowedSortFields = Enum
        .GetNames(typeof(AllowedSortFields))
        .Select(name => name.ToLower())
        .ToArray();

    public RequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page should be greater or equal to 1");

        RuleFor(x => x.PageSize)
            .NotNull()
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize should be between 1 and 100");

        RuleFor(x => x)
            .Must(x => !x.BirthDateFrom.HasValue || !x.BirthDateTo.HasValue || x.BirthDateFrom <= x.BirthDateTo)
            .WithMessage("BirthDateFrom must be less than or equal to BirthDateTo");

        RuleFor(x => x.GroupIds)
            .Must(list => list == null || list.All(id => id > 0))
            .WithMessage("GroupIds must contain positive integers");

        RuleFor(x => x.CourseIds)
            .Must(list => list == null || list.All(id => id > 0))
            .WithMessage("CourseIds must contain positive integers");

        RuleFor(x => x.LessonIds)
            .Must(list => list == null || list.All(id => id > 0))
            .WithMessage("LessonIds must contain positive integers");

        RuleFor(x => x.SortBy)
            .Must(sortBy => string.IsNullOrEmpty(sortBy) || AllowedSortFields.Contains(sortBy))
            .WithMessage($"SortBy must be one of the following: {string.Join(", ", AllowedSortFields)}");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must be 100 characters or less");
    }
}