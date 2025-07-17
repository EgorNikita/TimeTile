using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetCoursesBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidInstitutionEntityIdsList<GetCoursesBulkEndpoint.Request, Course>(db, institutionId);
                });
        }
    }
}
