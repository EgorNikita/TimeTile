using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetClassroomsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetClassroomsEndpoint.Request, AllowedSortFields>();

            // ClassroomTypeIds
            RuleFor(x => x.ClassroomTypeIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.ClassroomTypeIds != null, () => {
                        RuleFor(x => x.ClassroomTypeIds!)
                            .MustBeValidInstitutionEntityIdsList<GetClassroomsEndpoint.Request, ClassroomType>(db, institutionId);
                    });
                });
        }
    }
}
