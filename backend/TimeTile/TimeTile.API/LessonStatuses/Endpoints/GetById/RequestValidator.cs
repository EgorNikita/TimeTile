using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetLessonStatusByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db)
        {
            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<GetLessonStatusByIdEndpoint.Request, LessonStatus>(db);
                });
        }
    }
}
