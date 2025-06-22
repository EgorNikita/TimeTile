using FluentValidation;

namespace TimeTile.API.Common.Api.Pagination.PagedRequest;

public class PagedRequestValidator<T> : AbstractValidator<T>
    where T : IPagedRequest
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page should be greater or equal to 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(IPagedRequest.MaxPageSize)
            .WithMessage($"PageSize should be between 1 and {IPagedRequest.MaxPageSize}");
    }
}