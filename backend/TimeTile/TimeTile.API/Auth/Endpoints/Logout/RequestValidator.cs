using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Auth.Endpoints.Logout;

public class RequestValidator : AbstractValidator<Auth.Endpoints.Logout.Logout.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MustBeValidRefreshToken();
    }
}