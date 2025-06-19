using FluentValidation;
using TimeTile.API.Common;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.Regex;

namespace TimeTile.API.Students.Endpoints.Create;

public class RequestValidator : AbstractValidator<CreateStudentEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(u => u.Firstname)
            .MustBeValidName();

        RuleFor(u => u.Lastname)
            .MustBeValidName();

        RuleFor(u => u.BirthDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("BirthDate cannot be in the future.");

        RuleFor(u => u.PhoneNumber)
            .MustBeValidPhoneNumber();

        RuleFor(u => u.HomeAddress)
            .MustBeValidAddress();
    }
}