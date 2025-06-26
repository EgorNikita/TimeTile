using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using static TimeTile.Core.Common.Constants.Permissions;

namespace TimeTile.API.Roles.Endpoints.UpdatePermissions
{
    public class RequestBodyValidator : AbstractValidator<UpdatePermissionsEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db)
        {
            RuleFor(x => x.PermissionsToAdd)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.PermissionsToAdd != null, () =>
                    {
                        RuleFor(x => x.PermissionsToAdd!)
                            .MustBeValidEntityIdsList<UpdatePermissionsEndpoint.RequestBody, Permission>(db);
                    });
                    
                });

            RuleFor(x => x.PermissionsToRemove)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.PermissionsToRemove != null, () =>
                    {
                        RuleFor(x => x.PermissionsToRemove!)
                            .MustBeValidEntityIdsList<UpdatePermissionsEndpoint.RequestBody, Permission>(db);
                    });

                });
        }
    }
}
