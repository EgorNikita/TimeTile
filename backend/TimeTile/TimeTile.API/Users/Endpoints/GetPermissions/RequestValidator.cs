using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Endpoints.GetPermissions
{
    public class RequestValidator : PagedRequestValidator<GetUserPermissionsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetUserPermissionsEndpoint.Request, AllowedSortFields>();

            RuleFor(x => x.UserId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.UserId)
                        .MustBeValidOptionalInstitutionEntityId<GetUserPermissionsEndpoint.Request, User>(db, institutionId);
                });
        }
    }
}
