using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Courses.Endpoints.Create;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.UpdateCourseToStudent
{
    public class RequestParametersValidator : AbstractValidator<UpdateCourseToStudentEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.CourseId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.CourseId)
                        .MustBeValidInstitutionEntityId<UpdateCourseToStudentEndpoint.RequestParameters, Course>(db, institutionId);
                });

            RuleFor(x => x.StudentId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.StudentId)
                        .MustBeValidOptionalInstitutionEntityId<UpdateCourseToStudentEndpoint.RequestParameters, Student>(db, institutionId);
                });

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                {
                    return await db.CoursesStudents
                        .AsNoTracking()
                        .AnyAsync(cs =>
                            cs.CourseId == request.CourseId &&
                            cs.StudentId == request.StudentId, cancellationToken
                        );
                })
                .WithMessage("There is no relationship between student and course")
                .When(request =>
                {
                    var validator = new InlineValidator<UpdateCourseToStudentEndpoint.RequestParameters>();

                    validator.RuleFor(x => x.CourseId).MustBeValidId();
                    validator.RuleFor(x => x.StudentId).MustBeValidId();

                    var result = validator.Validate(request);
                    return result.IsValid;
                });
        }
    }
}
