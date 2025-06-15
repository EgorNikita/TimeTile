using FluentValidation;

namespace TimeTile.API.ClassroomTypes.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetClassroomTypeByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id should be greater or equal to 1");
        }
    }
}
