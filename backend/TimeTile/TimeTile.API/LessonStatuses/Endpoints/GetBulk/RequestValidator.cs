using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetLessonStatusesBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidInstitutionEntityIdsList<GetLessonStatusesBulkEndpoint.Request, LessonStatus>(db, institutionId);
                });
        }
    }
}
