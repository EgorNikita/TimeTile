using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.ClassroomTypes.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetClassroomTypeByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeValidId();
        }
    }
}
