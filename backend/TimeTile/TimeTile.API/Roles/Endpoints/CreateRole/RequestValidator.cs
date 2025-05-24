using FluentValidation;

namespace TimeTile.API.Roles.Endpoints.CreateRole;

public class RequestValidator : AbstractValidator<CreateRoleEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.PermissionsIds)
            .NotNull().WithMessage("PermissionsIds must be provided.")
            .Must(list => list.Count > 0).WithMessage("At least one permission ID must be specified.")
            .ForEach(id => id.GreaterThan(0).WithMessage("Permission IDs must be positive integers."));
    }
}