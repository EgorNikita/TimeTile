using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.GetById;

public class RequestValidator : AbstractValidator<GetInstitutionByIdEndpoint.Request>
{
    public RequestValidator(TimetileDbContext db)
    {
        RuleFor(x => x.Id)
            .MustBeValidId()
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .MustBeValidEntityId<GetInstitutionByIdEndpoint.Request, Institution>(db);
            });
    }
}