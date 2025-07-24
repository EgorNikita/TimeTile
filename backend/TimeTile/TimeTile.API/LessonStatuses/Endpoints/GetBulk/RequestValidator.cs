using FluentValidation;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.LessonStatuses.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetLessonStatusesBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db)
        {
            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidEntityIdsList<GetLessonStatusesBulkEndpoint.Request, LessonStatus>(db);
                });
        }
    }
}
