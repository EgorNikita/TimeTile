using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.GetById
{
    public class GetLessonByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns Lesson by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find Lesson
            var lesson = await db.Lessons
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            var response = new Response(
                lesson.Id,
                lesson.TimetableUnitId,
                lesson.CourseId,
                lesson.ClassroomId,
                lesson.LessonStatusId,
                lesson.Date,
                lesson.Description,
                lesson.AssignmentId
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            int Id,
            int TimetableUnitId,
            int CourseId,
            int ClassroomId,
            int LessonStatusId,
            DateTimeOffset Date,
            string Description,
            int? AssignmentId
        );
    }
}
