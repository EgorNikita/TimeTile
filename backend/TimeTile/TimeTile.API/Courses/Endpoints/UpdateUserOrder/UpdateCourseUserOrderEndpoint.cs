using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.UpdateUserOrder
{
    public class UpdateCourseUserOrderEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPut("/{Id:int}/user-order", Handle)
                .RequireUserId()
                .WithSummary("Update of user order of Courses")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            ICourseService courseService,
            TimetileDbContext db,
            IUserProvider userProvider,
            CancellationToken cancellationToken)
        {
            int userId = userProvider.GetUserId();

            var entity = await db.CoursesUsers
                .Include(cu => cu.Course)
                .FirstAsync(cu => cu.UserId == userId && cu.CourseId == parameters.Id, cancellationToken);

            var currentValue = entity.OrderNumber;

            if (body.NewOrderNumber != currentValue)
            {
                entity.OrderNumber = int.MaxValue;
                await db.SaveChangesAsync(cancellationToken);

                if (body.NewOrderNumber > currentValue)
                {
                    var relationsBetween = await db.CoursesUsers
                        .Where(cu => cu.UserId == userId &&
                            cu.OrderNumber > currentValue &&
                            cu.OrderNumber <= body.NewOrderNumber)
                        .ToListAsync(cancellationToken);

                    relationsBetween.ForEach(r => r.OrderNumber -= 1);
                }
                else if (body.NewOrderNumber < currentValue)
                {
                    var relationsBetween = await db.CoursesUsers
                        .Where(cu => cu.UserId == userId &&
                            cu.OrderNumber >= body.NewOrderNumber &&
                            cu.OrderNumber < currentValue)
                        .ToListAsync(cancellationToken);

                    relationsBetween.ForEach(r => r.OrderNumber += 1);
                }

                await db.SaveChangesAsync(cancellationToken);

                entity.OrderNumber = body.NewOrderNumber;
                await db.SaveChangesAsync(cancellationToken);
            }

            var course = entity.Course;

            // Return result
            var response = new Response(
                course.Id,
                course.Title,
                course.SubjectId,
                course.TeacherId,
                course.IsAdvanced,
                course.TermId,
                entity.OrderNumber,
                courseService.GetIconUrl(course)
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record RequestParameters(
            int Id
        ); 

        public sealed record RequestBody(
            int NewOrderNumber
        );

        private sealed record Response(
            int Id,
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId,
            int OrderNumber,
            string IconUrl
        );
    }
}
