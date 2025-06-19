using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Roles.Endpoints.CreateRole;

public class RequestValidator : AbstractValidator<CreateRoleEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .MustBeValidTitle();

        RuleFor(x => x.PermissionsIds)          // TODO: change logic
            .NotNull().WithMessage("PermissionsIds must be provided.")
            .Must(list => list.Count > 0).WithMessage("At least one permission ID must be specified.")
            .ForEach(id => id.GreaterThan(0).WithMessage("Permission IDs must be positive integers."));
    }
}