using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Auth.Endpoints.Login;

public class RequestValidator : AbstractValidator<Auth.Endpoints.Login.Login.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required.")
            .MustBeValidEmail();
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}