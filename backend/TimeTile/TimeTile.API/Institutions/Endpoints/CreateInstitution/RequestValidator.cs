using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Institutions.Endpoints.CreateInstitution;

public class RequestValidator : AbstractValidator<CreateInstitutionEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .MustBeValidTitle();

        RuleFor(x => x.Address)
            .MustBeValidAddress();

        RuleFor(x => x.PhoneNumber)
            .MustBeValidPhoneNumber();

        RuleFor(x => x.Email)
            .MustBeValidEmail();

        RuleFor(x => x.Domain)
            .MustBeValidString()
            .WithMessage("Domain contains invalid characters.")
            .ApplyRegexPattern(RegexPatterns.Pattern.Domain);
    }
}