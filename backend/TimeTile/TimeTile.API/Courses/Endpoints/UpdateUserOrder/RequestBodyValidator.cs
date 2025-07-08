using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api.Http;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.UpdateUserOrder
{
    public class RequestBodyValidator : AbstractValidator<UpdateCourseUserOrderEndpoint.RequestBody>
    {
        public RequestBodyValidator(TimetileDbContext db, IUserProvider userProvider)
        {
            var userId = userProvider.GetUserId();

            RuleFor(x => x.NewOrderNumber)
                .GreaterThan(0)
                .WithMessage("OrderNumber should be positive.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.NewOrderNumber)
                        .MustAsync(async (newOrderNumber, cancellationToken) =>
                        {
                            return await db.CoursesUsers
                                .Where(cu => cu.UserId == userId)
                                .AnyAsync(cu => cu.OrderNumber >= newOrderNumber, cancellationToken);
                        })
                        .WithMessage("OrderNumber is invalid");
                });
        }
    }
}
