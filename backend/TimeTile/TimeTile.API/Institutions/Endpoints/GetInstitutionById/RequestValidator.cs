using FluentValidation;

namespace TimeTile.API.Institutions.Endpoints.GetInstitutionById
{
    public class RequestValidator : AbstractValidator<GetInstitutionByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id should be greater or equal to 1");
        }
    }
}
