using FluentValidation;
using TimeTile.API.Common;

namespace TimeTile.API.Roles.Endpoints.CreateRole;

public class RequestValidator : AbstractValidator<CreateRoleEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(x => string.IsNullOrEmpty(x) || InputSanitizer.Sanitize(x) == x)
            .WithMessage("Title contains invalid characters.")
            .ApplyRegexPattern("Title");

        RuleFor(x => x.PermissionsIds)
            .NotNull().WithMessage("PermissionsIds must be provided.")
            .Must(list => list.Count > 0).WithMessage("At least one permission ID must be specified.")
            .ForEach(id => id.GreaterThan(0).WithMessage("Permission IDs must be positive integers."));
    }
}