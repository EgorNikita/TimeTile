using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;

namespace TimeTile.API.Students.Endpoints.Get;

public class RequestValidator : PagedRequestValidator<GetStudentsEndpoint.Request>
{
    public RequestValidator()           // TODO: change logic of validation
    {
        RuleFor(x => x)
            .Must(x => !x.BirthDateFrom.HasValue || !x.BirthDateTo.HasValue || x.BirthDateFrom <= x.BirthDateTo)
            .WithMessage("BirthDateFrom must be less than or equal to BirthDateTo");

        RuleFor(x => x.GroupIds)
            .MustBeValidOptionalListOfIds();

        RuleFor(x => x.CourseIds)
            .MustBeValidOptionalListOfIds();

        RuleFor(x => x.LessonIds)
            .MustBeValidOptionalListOfIds();

        RuleFor(x => x.SortBy)
            .MustBeValidSortField<GetStudentsEndpoint.Request, AllowedSortFields>();

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term must be 100 characters or less");
    }
}