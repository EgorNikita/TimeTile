using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeTile.API.Common.Api;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.API.Common.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using static TimeTile.API.Endpoints;

namespace TimeTile.API.Lessons.Endpoints.Create
{
    public class CreateLessonEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new Lesson")
                .WithRequestValidation<Request>();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Save Lesson
            var lesson = new Lesson
            {
                CourseId = request.CourseId,
                ClassroomId = request.ClassroomId,
                LessonStatusId = request.LessonStatusId 
                    ?? (await db.LessonStatuses.FirstAsync(ls => ls.Description == Core.Common.Constants.LessonStatuses.Scheduled)).Id,
                Date = request.Date.ToUniversalTime(),
                Description = request.Description.Trim(),
                LessonToTimetableUnits = request.TimetableUnitIds
                    .Select(id => new LessonToTimetableUnit { TimetableUnitId = id })
                    .ToList(),
                LessonsToStudents = db.CoursesStudents
                    .Where(cs => cs.CourseId == request.CourseId)
                    .Select(cs => new LessonToStudent { StudentId = cs.StudentId })
                    .ToList()
            };

            await db.Lessons.AddAsync(lesson, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                lesson.Id,
                lesson.LessonToTimetableUnits.Select(lt => lt.TimetableUnitId).ToArray(),
                lesson.CourseId,
                lesson.ClassroomId,
                lesson.LessonStatusId,
                lesson.Date,
                lesson.Description,
                lesson.AssignmentId
            );

            var result = Result.Success(response);

            return TypedResults.Created($"{Routes.Lessons}/{lesson.Id}", result);
        }

        public sealed record Request(
            List<int> TimetableUnitIds,
            int CourseId,
            int ClassroomId,
            int? LessonStatusId,
            DateTimeOffset Date,
            string Description
        );

        private sealed record Response(
            int Id,
            int[] TimetableUnitIds,
            int CourseId,
            int ClassroomId,
            int LessonStatusId,
            DateTimeOffset Date,
            string Description,
            int? AssignmentId
        );
    }
}
