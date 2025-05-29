using FluentValidation;

namespace TimeTile.API.Institutions.Endpoints.CreateInstitution;

public class RequestValidator : AbstractValidator<CreateInstitutionEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 format
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("Domain is required.")
            .Matches(@"^(?!-)[A-Za-z0-9-]{1,63}(?<!-)(\.[A-Za-z]{2,})+$")
            .WithMessage("Invalid domain format.");
    }
}