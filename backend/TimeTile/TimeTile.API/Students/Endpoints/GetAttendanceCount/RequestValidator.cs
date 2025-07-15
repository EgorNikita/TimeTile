using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetAttendanceCount
{
    public class RequestValidator : AbstractValidator<GetAttendanceCountEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidOptionalInstitutionEntityId<GetAttendanceCountEndpoint.Request, Student>(db, institutionId);
                });

            // CourseIds
            RuleFor(x => x.CourseIds)
                .MustBeValidOptionalListOfIds()
                .DependentRules(() =>
                {
                    When(x => x.CourseIds != null, () => {
                        RuleFor(x => x.CourseIds!)
                            .MustBeValidInstitutionEntityIdsList<GetAttendanceCountEndpoint.Request, Course>(db, institutionId);
                    });
                });
        }
    }
}
