using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Subjects.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetSubjectsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetSubjectsEndpoint.Request, AllowedSortFields>();

            // TeacherIds
            RuleFor(x => x.TeacherIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.TeacherIds != null, () => {
                        RuleFor(x => x.TeacherIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetSubjectsEndpoint.Request, InstitutionMember>(db, institutionId);
                    });
                });
        }
    }
}
