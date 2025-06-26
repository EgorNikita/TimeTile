using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Grades.Endpoints.GetById
{
    public class RequestValidator : AbstractValidator<GetGradeByIdEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidEntityId<GetGradeByIdEndpoint.Request, Grade>(db);
                })
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustAsync(async (id, cancellationToken) =>
                        {
                            var grade = await db.Grades
                                .Include(g => g.LessonToStudentClasswork)
                                    .ThenInclude(cw => cw.Student)
                                .Include(g => g.LessonToStudentHomework)
                                    .ThenInclude(hw => hw.Student)
                                .Include(g => g.CourseToStudent)
                                    .ThenInclude(cs => cs.Course)
                                .FirstAsync(g => g.Id == id, cancellationToken);

                            return grade.Type switch
                            {
                                GradeType.Classwork => grade.LessonToStudentClasswork!.Student.InstitutionId == institutionId,
                                GradeType.Homework => grade.LessonToStudentHomework!.Student.InstitutionId == institutionId,
                                GradeType.Exam => grade.CourseToStudent!.Course.InstitutionId == institutionId,
                                _ => false,
                            };
                        })
                        .WithMessage("Grade does not belong to the current institution.");
                });
        }
    }
}
