using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.UpdateLessonToStudent
{
    public class RequestParametersValidator : AbstractValidator<UpdateLessonToStudentEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.LessonId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.LessonId)
                        .MustBeValidEntityId<UpdateLessonToStudentEndpoint.RequestParameters, Lesson>(db);
                });

            RuleFor(x => x.StudentId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.StudentId)
                        .MustBeValidOptionalInstitutionEntityId<UpdateLessonToStudentEndpoint.RequestParameters, Student>(db, institutionId);
                });

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                {
                    return await db.LessonsStudents
                        .AsNoTracking()
                        .AnyAsync(ls =>
                            ls.LessonId == request.LessonId &&
                            ls.StudentId == request.StudentId, cancellationToken
                        );
                })
                .WithMessage("There is no relationship between student and lesson")
                .When(request =>
                {
                    var validator = new InlineValidator<UpdateLessonToStudentEndpoint.RequestParameters>();

                    validator.RuleFor(x => x.LessonId).MustBeValidId();
                    validator.RuleFor(x => x.StudentId).MustBeValidId();

                    var result = validator.Validate(request);
                    return result.IsValid;
                });
        }
    }
}
