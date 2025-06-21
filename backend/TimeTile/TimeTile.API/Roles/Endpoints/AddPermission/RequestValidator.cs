using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.AddPermission
{
    public class RequestValidator : AbstractValidator<AddPermissionToRoleEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.RoleId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleId)
                        .MustBeValidOptionalInstitutionEntityId<AddPermissionToRoleEndpoint.Request, Role>(db, institutionId);
                });

            RuleFor(x => x.PermissionId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.PermissionId)
                        .MustBeValidEntityId<AddPermissionToRoleEndpoint.Request, Permission>(db);
                });
        }
    }
}
