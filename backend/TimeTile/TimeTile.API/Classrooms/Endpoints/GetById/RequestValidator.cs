using FluentValidation;

namespace TimeTile.API.Classrooms.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetClassroomByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id should be greater or equal to 1");
        }
    }
}
