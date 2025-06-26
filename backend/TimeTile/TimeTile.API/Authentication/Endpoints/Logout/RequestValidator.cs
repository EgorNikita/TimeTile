using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Authentication.Endpoints.Logout;

public class RequestValidator : AbstractValidator<Logout.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MustBeValidRefreshToken();
    }
}