using FluentValidation;

namespace TimeTile.API.Students.Endpoints.GetStudentById;

public class RequestValidator : AbstractValidator<GetStudentByIdEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id should be greater or equal to 1");
    }
}