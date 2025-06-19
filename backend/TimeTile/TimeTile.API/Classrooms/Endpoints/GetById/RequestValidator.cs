using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Classrooms.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetClassroomByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeValidId();
        }
    }
}
