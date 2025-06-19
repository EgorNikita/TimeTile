using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.TimetableUnits.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetTimetableUnitByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeValidId();
        }
    }
}
