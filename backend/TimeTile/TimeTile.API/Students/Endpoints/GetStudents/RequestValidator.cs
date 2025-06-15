using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Students.Endpoints.GetStudents;

public class RequestValidator : PagedRequestValidator<GetStudentsEndpoint.Request>
{
    public RequestValidator()
    {
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
            .MustBeValidSortField<GetStudentsEndpoint.Request, AllowedSortFields>();

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must be 100 characters or less");
    }
}