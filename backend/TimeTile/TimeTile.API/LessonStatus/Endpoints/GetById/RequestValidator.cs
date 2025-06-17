using FluentValidation;

namespace TimeTile.API.LessonStatus.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetLessonStatusByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id should be greater or equal to 1");
        }
    }
}
