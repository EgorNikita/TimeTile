using TimeTile.API.Classrooms.Endpoints.GetById;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Roles.Endpoints.GetPermissions
{
    public class RequestValidator : PagedRequestValidator<GetRolePermissionsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetRolePermissionsEndpoint.Request, AllowedSortFields>();

            RuleFor(x => x.RoleId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleId)
                            .MustBeValidOptionalInstitutionEntityId<GetRolePermissionsEndpoint.Request, Role>(db, institutionId);
                });
        }
    }
}
