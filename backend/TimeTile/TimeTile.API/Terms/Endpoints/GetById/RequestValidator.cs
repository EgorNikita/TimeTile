using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Terms.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetTermByIdEndpoint.Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeValidId();
        }
    }
}
