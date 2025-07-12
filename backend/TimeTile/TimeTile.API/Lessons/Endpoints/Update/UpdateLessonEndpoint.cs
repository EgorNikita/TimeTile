using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeTile.API.Common.Api;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TimeTile.API.Common.Api.Extensions;

namespace TimeTile.API.Lessons.Endpoints.Update
{
    public class UpdateLessonEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPatch("/{Id:int}", Handle)
                .WithSummary("Partial update of Lesson")
                .WithRequestValidation<RequestParameters>()
                .WithRequestValidation<RequestBody>();
        }

        private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
            [AsParameters] RequestParameters parameters,
            [FromBody] RequestBody body,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var lesson = await db.Lessons
                .Include(l => l.LessonsToStudents)
                .FirstAsync(l => l.Id == parameters.Id, cancellationToken);

            await UpdateEntity(lesson, body, db, cancellationToken);

            if (await IsDuplicate(lesson, db, cancellationToken))
            {
                var error = Error.From("Lesson with such data already exists");
                var failure = Result.Failure(error);

                return TypedResults.BadRequest(failure);
            }

            await db.SaveChangesAsync(cancellationToken);

            // Return result
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

        private static async Task UpdateEntity(
            Lesson lesson,
            RequestBody request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            if (request.TimetableUnitId is not null)
                lesson.TimetableUnitId = request.TimetableUnitId.Value;

            if (request.CourseId is not null)
            {
                db.LessonsStudents.RemoveRange(lesson.LessonsToStudents);

                lesson.CourseId = request.CourseId.Value;

                lesson.LessonsToStudents = await db.CoursesStudents
                    .Where(cs => cs.CourseId == request.CourseId.Value)
                    .Select(cs => new LessonToStudent { StudentId = cs.StudentId })
                    .ToListAsync(cancellationToken);
            }

            if (request.ClassroomId is not null)
                lesson.ClassroomId = request.ClassroomId.Value;

            if (request.LessonStatusId is not null)
                lesson.LessonStatusId = request.LessonStatusId.Value;

            if (request.Date is not null)
                lesson.Date = request.Date.Value.ToUniversalTime();

            if (request.Description is not null)
                lesson.Description = request.Description.Trim();
        }

        private static async Task<bool> IsDuplicate(Lesson lesson, TimetileDbContext db, CancellationToken cancellationToken)
        {
            return await db.Lessons
                .AsNoTracking()
                .AnyAsync(l =>
                    l.Id != lesson.Id &&
                    l.CourseId == lesson.CourseId &&
                    l.TimetableUnitId == lesson.TimetableUnitId &&
                    l.Date == lesson.Date,
                    cancellationToken
                );
        }

        public sealed record RequestParameters(
            int Id
        );

        public sealed record RequestBody(
            int? TimetableUnitId,
            int? CourseId,
            int? ClassroomId,
            int? LessonStatusId,
            DateTimeOffset? Date,
            string? Description
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
