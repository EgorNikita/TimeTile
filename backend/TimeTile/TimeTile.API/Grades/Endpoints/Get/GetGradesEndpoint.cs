using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Grades.Endpoints.Get
{
    public class GetGradesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of grades")
                .WithRequestValidation<Request>()
                .RequireAuthorization(Permissions.Grades.Get);
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var grades = await BuildFilteredQuery(request, db)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
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
                                : x.Submission!.Assignment.Lesson.Course
                })
                .Select(x => new Response(
                    x.Id,
                    x.Value,
                    x.Weight,
                    x.Type,
                    x.Date,
                    x.Course.SubjectId,
                    x.Course.Id
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(grades);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Grade> BuildFilteredQuery(Request request, TimetileDbContext db)
        {
            var baseQuery = db.Grades
                .AsNoTracking();

            if (request.Types is not null && request.Types.Any())
            {
                var gradeTypes = request.Types
                    .Select(t => Enum.Parse<GradeType>(t, ignoreCase: true))
                    .ToArray();

                baseQuery = baseQuery.Where(g =>
                    gradeTypes.Contains(g.Type)
                );
            }

            if (request.LessonIds is not null && request.LessonIds.Any())
                baseQuery = baseQuery.Where(g =>
                    (g.LessonToStudent != null && request.LessonIds.Contains(g.LessonToStudent.LessonId))
                    || (g.Submission != null && request.LessonIds.Contains(g.Submission.Assignment.Lesson.Id))
                );

            if (request.StudentIds is not null && request.StudentIds.Any())
                baseQuery = baseQuery.Where(g =>
                    (g.LessonToStudent != null && request.StudentIds.Contains(g.LessonToStudent.StudentId))
                    || (g.Submission != null && request.StudentIds.Contains(g.Submission.StudentId))
                    || (g.CourseToStudent != null && request.StudentIds.Contains(g.CourseToStudent.StudentId))
                );

            // Find grades either directly associated with courses as grades for exam OR
            // grades for lessons within those courses
            if (request.CourseIds is not null && request.CourseIds.Any())
                baseQuery = baseQuery.Where(g =>
                    (g.CourseToStudent != null && request.CourseIds.Contains(g.CourseToStudent.CourseId))       // direct grades
                    || (db.Lessons
                        .Where(l => request.CourseIds.Contains(l.CourseId))     // Filtration by lesson associated with course
                        .Any(l => l.LessonsToStudents.Any(ls => ls.GradeId == g.Id) ||      // Grade for Classwork
                            (l.Assignment != null && l.Assignment.Submissions.Any(s => s.GradeId == g.Id))))        // Grade for Submission(Homework)
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? LessonIds = null,
            int[]? StudentIds = null,
            int[]? CourseIds = null,
            string[]? Types = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByStudentsRequest;

        private sealed record Response(
            int Id,
            short Value,
            float Weight,
            string Type,
            DateTimeOffset Date,
            int SubjectId,
            int CourseId
        );
    }
}
