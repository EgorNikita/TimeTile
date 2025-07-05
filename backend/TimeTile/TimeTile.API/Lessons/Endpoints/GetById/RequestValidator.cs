using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetLessonByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<GetLessonByIdEndpoint.Request, Lesson>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Id)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    var lesson = await db.Lessons
                                        .Include(l => l.Course)
                                        .FirstAsync(l => l.Id == id, cancellationToken);

                                    return lesson.Course.InstitutionId == institutionId;
                                })
                                .WithMessage("There is no Lesson with this id in your current institution");
                        });
                });
        }
    }
}
