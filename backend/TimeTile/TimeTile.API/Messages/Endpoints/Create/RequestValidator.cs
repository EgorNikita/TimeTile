using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Endpoints.Create
{
    public class RequestValidator : AbstractValidator<CreateMessageEndpoint.Request>
    {
        public RequestValidator(TimetileDbContext db, IInstitutionProvider institutionProvider, IUserProvider userProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userId = userProvider.GetUserId();

            RuleFor(x => x.CourseId)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.CourseId)
                        .MustBeValidInstitutionEntityId<CreateMessageEndpoint.Request, Course>(db, institutionId)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.CourseId)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    var course = await db.Courses
                                        .Include(c => c.CoursesToStudents)
                                        .FirstAsync(c => c.Id == id, cancellationToken);

                                    return course.TeacherId == userId || 
                                        course.CoursesToStudents.Any(cs => cs.StudentId == userId);
                                })
                                .WithMessage("You are not part of the course. You are not able to create new messages.");
                        });
                });

            RuleFor(x => x)
                .Must(request => request.Content != null || request.Files?.Any() == true)
                .WithMessage("Message must have either content or files attached.");
        }
    }
}
