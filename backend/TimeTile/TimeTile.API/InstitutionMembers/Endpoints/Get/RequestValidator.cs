using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.InstitutionMembers.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetInstitutionMembersEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetInstitutionMembersEndpoint.Request, AllowedSortFields>();

            // SubjectIds
            RuleFor(x => x.SubjectIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.SubjectIds != null, () => {
                        RuleFor(x => x.SubjectIds!)
                            .MustBeValidInstitutionEntityIdsList<GetInstitutionMembersEndpoint.Request, Subject>(db, institutionId);
                    });
                });

            // PreferableClassroomIds
            RuleFor(x => x.PreferredClassroomIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.PreferredClassroomIds != null, () => {
                        RuleFor(x => x.PreferredClassroomIds!)
                            .MustBeValidInstitutionEntityIdsList<GetInstitutionMembersEndpoint.Request, Classroom>(db, institutionId);
                    });
                });

            // RoleIds
            RuleFor(x => x.RoleIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.RoleIds != null, () => {
                        RuleFor(x => x.RoleIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetInstitutionMembersEndpoint.Request, Role>(db, institutionId);
                    });
                });

            // GroupIds
            RuleFor(x => x.GroupIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.GroupIds != null, () => {
                        RuleFor(x => x.GroupIds!)
                            .MustBeValidInstitutionEntityIdsList<GetInstitutionMembersEndpoint.Request, Group>(db, institutionId);
                    });
                });
        }
    }
}
