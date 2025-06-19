using FluentValidation;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Institutions.Endpoints.GetInstitutionById;

public class RequestValidator : AbstractValidator<GetInstitutionByIdEndpoint.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .MustBeValidId();
    }
}