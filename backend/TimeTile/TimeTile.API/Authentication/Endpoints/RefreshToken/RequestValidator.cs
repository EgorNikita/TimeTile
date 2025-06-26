using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Authentication.Endpoints.RefreshToken;

public class RequestValidator : AbstractValidator<RefreshToken.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MustBeValidRefreshToken();
    }
}