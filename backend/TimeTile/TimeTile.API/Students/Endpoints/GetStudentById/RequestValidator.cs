using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Students.Endpoints.GetStudentById;

public class RequestValidator : AbstractValidator<GetStudentByIdEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .MustBeValidId();
    }
}