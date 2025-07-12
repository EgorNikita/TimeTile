using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Students.Endpoints.GetCourses;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetLessons
{
    public class RequestValidator : AbstractValidator<GetStudentLessonsEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.SortBy)
                .MustBeValidSortField<GetStudentLessonsEndpoint.Request, AllowedSortFields>();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidOptionalInstitutionEntityId<GetStudentLessonsEndpoint.Request, Student>(db, institutionId);
                });
        }
    }
}
