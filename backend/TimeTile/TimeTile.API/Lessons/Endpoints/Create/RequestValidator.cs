using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateLessonEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.TimetableUnitIds)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.TimetableUnitIds)
                        .MustBeValidInstitutionEntityIdsList<CreateLessonEndpoint.Request, TimetableUnit>(db, institutionId);
                });

            RuleFor(x => x.CourseId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.CourseId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, Course>(db, institutionId);
                });

            RuleFor(x => x.ClassroomId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.ClassroomId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, Classroom>(db, institutionId);
                });

            RuleFor(x => x.LessonStatusId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.LessonStatusId)
                        .MustBeValidInstitutionEntityId<CreateLessonEndpoint.Request, LessonStatus>(db, institutionId);
                });

            RuleFor(x => x.Date)
                .Must(date => date >= DateTimeOffset.UtcNow)
                .WithMessage("Date should be from future");

            RuleFor(x => x.Description)
                .MustBeValidDescription();

            // Unique constraint
            RuleFor(x => x)
               .MustAsync(async (request, cancellationToken) =>
               {
                   var date = request.Date.ToUniversalTime();

                    return !await db.Lessons
                        .AsNoTracking()
                        .AnyAsync(l =>
                            l.CourseId == request.CourseId &&
                            l.Date == date &&
                            l.LessonToTimetableUnits.Any(lt => request.TimetableUnitIds.Contains(lt.TimetableUnitId)),
                            cancellationToken
                        );
               })
               .WithMessage("Lesson with such data already exists")
               // Call to the database only in case of successfull validation before
               .When(request =>
               {
                   var validator = new InlineValidator<CreateLessonEndpoint.Request>();

                   validator.RuleFor(x => x.CourseId).MustBeValidId();
                   validator.RuleFor(x => x.TimetableUnitIds).MustBeValidListOfIds();
                   validator.RuleFor(x => x.Date).Must(date => date >= DateTimeOffset.UtcNow);

                   var result = validator.Validate(request);
                   return result.IsValid;
               });
        }
    }
}
