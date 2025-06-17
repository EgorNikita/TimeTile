using FluentValidation;

namespace TimeTile.API.Term.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetTermByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id should be greater or equal to 1");
        }
    }
}
