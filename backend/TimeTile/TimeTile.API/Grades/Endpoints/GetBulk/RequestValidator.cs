using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Grades.Endpoints.GetById;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Grades.Endpoints.GetBulk
{
    public class RequestValidator : AbstractValidator<GetGradesBulkEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            RuleFor(x => x.Ids)
                .MustBeValidListOfIds()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Ids)
                        .MustBeValidEntityIdsList<GetGradesBulkEndpoint.Request, Grade>(db)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Ids)
                                .MustAsync(async (ids, cancellationToken) =>
                                {
                                    return await db.Grades
                                        .AnyAsync(g =>
                                            (
                                                g.Type == GradeType.Classwork && 
                                                g.LessonToStudent!.Student.InstitutionId != institutionId
                                            ) || (
                                                g.Type == GradeType.Homework && 
                                                g.Submission!.Student.InstitutionId != institutionId
                                            ) || (
                                                g.Type == GradeType.Exam &&
                                                g.CourseToStudent!.Course.InstitutionId != institutionId
                                            ), cancellationToken);
                                })
                                .WithMessage("Some of Grades do not belong to the current institution.");
                        });
                });
        }
    }
}
