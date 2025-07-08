using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.UpdateUserOrder
{
    public class RequestParametersValidator : AbstractValidator<UpdateCourseUserOrderEndpoint.RequestParameters>
    {
        public RequestParametersValidator(TimetileDbContext db, IInstitutionProvider institutionProvider, IUserProvider userProvider)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userId = userProvider.GetUserId();

            RuleFor(x => x.Id)
                .MustBeValidId()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Id)
                        .MustBeValidInstitutionEntityId<UpdateCourseUserOrderEndpoint.RequestParameters, Course>(db, institutionId)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Id)
                                .MustAsync(async (id, cancellationToken) =>
                                {
                                    return await db.CoursesUsers
                                        .AnyAsync(cu => cu.CourseId == id && cu.UserId == userId);
                                })
                                .WithMessage("Course is not associated with current user");
                        });
                });
        }
    }
}
