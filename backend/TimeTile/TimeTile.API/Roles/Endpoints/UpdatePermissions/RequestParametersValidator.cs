using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.UpdatePermissions
{
    public class RequestParametersValidator : AbstractValidator<UpdatePermissionsEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.RoleId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleId)
                        .MustBeValidOptionalInstitutionEntityId<UpdatePermissionsEndpoint.RequestParameters, Role>(db, institutionId);
                });
        }
    }
}
