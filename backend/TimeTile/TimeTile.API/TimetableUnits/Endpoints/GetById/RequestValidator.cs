using FluentValidation;

namespace TimeTile.API.TimetableUnits.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetTimetableUnitByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id should be greater or equal to 1");
        }
    }
}
