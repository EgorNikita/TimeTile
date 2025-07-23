using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Grades.Endpoints.GetBulk
{
    public class GetGradesBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns grades by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var grades = await db.Grades
                .AsNoTracking()
                .Where(g => request.Ids.Contains(g.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Value,
                    x.Weight,
                    Type = x.Type.ToString(),
                    Date = x.UpdatedAt,
                    Course = x.CourseToStudent != null
                            ? x.CourseToStudent.Course
                            : x.LessonToStudent != null
                                ? x.LessonToStudent.Lesson.Course
                                : x.Submission!.Assignment.Lesson.Course,
                    LessonId = x.LessonToStudent != null
                        ? x.LessonToStudent.LessonId
                        : x.Submission != null
                            ? x.Submission!.Assignment.Lesson.Id
                            : (int?)null
                })
                .Select(x => new Response(
                    x.Id,
                    x.Value,
                    x.Weight,
                    x.Type,
                    x.Date,
                    x.Course.SubjectId,
                    x.Course.Id,
                    x.LessonId
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(grades);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

        private sealed record Response(
            int Id,
            float Value,
            float Weight,
            string Type,
            DateTimeOffset Date,
            int SubjectId,
            int CourseId,
            int? LessonId
        );
    }
}
