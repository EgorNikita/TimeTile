using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetLessonStatusByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .MustBeValidInstitutionEntityId<GetLessonStatusByIdEndpoint.Request, LessonStatus>(db, institutionId);
        }
    }
}
