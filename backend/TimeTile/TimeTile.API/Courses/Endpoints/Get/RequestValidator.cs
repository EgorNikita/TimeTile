using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Groups.Endpoints.Get;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.Get
{
    public class RequestValidator : PagedRequestValidator<GetCoursesEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetCoursesEndpoint.Request, AllowedSortFields>();

            // SubjectIds
            RuleFor(x => x.SubjectIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.SubjectIds != null, () => {
                        RuleFor(x => x.SubjectIds!)
                            .MustBeValidInstitutionEntityIdsList<GetCoursesEndpoint.Request, Subject>(db, institutionId);
                    });
                });

            // TeacherIds
            RuleFor(x => x.TeacherIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.TeacherIds != null, () => {
                        RuleFor(x => x.TeacherIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetCoursesEndpoint.Request, InstitutionMember>(db, institutionId);
                    });
                });

            // TermIds
            RuleFor(x => x.TermIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.TermIds != null, () => {
                        RuleFor(x => x.TermIds!)
                            .MustBeValidInstitutionEntityIdsList<GetCoursesEndpoint.Request, Term>(db, institutionId);
                    });
                });

            // StudentIds
            RuleFor(x => x.StudentIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.StudentIds != null, () => {
                        RuleFor(x => x.StudentIds!)
                            .MustBeValidOptionalInstitutionEntityIdsList<GetCoursesEndpoint.Request, Student>(db, institutionId);
                    });
                });

            // GroupIds
            RuleFor(x => x.GroupIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.GroupIds != null, () => {
                        RuleFor(x => x.GroupIds!)
                            .MustBeValidInstitutionEntityIdsList<GetCoursesEndpoint.Request, Group>(db, institutionId);
                    });
                });
        }
    }
}
