using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.RemovePermission
{
    public class RequestValidator : AbstractValidator<RemovePermissionFromRoleEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.RoleId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleId)
                        .MustBeValidOptionalInstitutionEntityId<RemovePermissionFromRoleEndpoint.Request, Role>(db, institutionId);
                });

            RuleFor(x => x.PermissionId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.PermissionId)
                        .MustBeValidEntityId<RemovePermissionFromRoleEndpoint.Request, Permission>(db);
                });
        }
    }
}
