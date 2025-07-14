using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetCourses
{
    public class GetStudentCoursesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{Id:int}/courses", Handle)
                .WithSummary("Gets Courses of the Student")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            ICourseService courseService,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relations = await BuildFilteredQuery(request, request.Id, db)
                .Include(cs => cs.Course)
                    .ThenInclude(c => c.Icon)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(cs => new {
                    cs.CourseId,
                    cs.Course,
                    cs.ExamGradeId,
                    cs.HasExam,
                    cs.PositionX,
                    cs.PositionY
                })
                .ToPagedListAsync(request, cancellationToken);

            var responses = new List<Response>();

            foreach (var item in relations.Items)
            {
                var averageGrade = await CalculateAverageGrade(request.Id, item.CourseId, db, cancellationToken);

                responses.Add(new Response(
                    item.CourseId,
                    new CourseInfo(
                        item.Course.Id,
                        item.Course.Title,
                        item.Course.SubjectId,
                        item.Course.TeacherId,
                        item.Course.IsAdvanced,
                        item.Course.TermId,
                        courseService.GetIconUrl(item.Course)
                    ),
                    item.ExamGradeId,
                    item.HasExam,
                    item.PositionX,
                    item.PositionY,
                    averageGrade
                ));
            }

            var result = Result.Success(new PagedList<Response>(
                responses,
                relations.Page,
                relations.PageSize,
                relations.TotalPages,
                relations.TotalCount
            ));

            return TypedResults.Ok(result);
        }

        private static async Task<float> CalculateAverageGrade(int studentId, int courseId, TimetileDbContext db, CancellationToken cancellationToken)
        {
            var submissionGradeIds = await db.Submissions
                .Where(s => s.GradeId != null && s.StudentId == studentId && s.Assignment.Lesson.CourseId == courseId)
                .Select(s => s.GradeId!.Value)
                .ToListAsync(cancellationToken);

            var classworkGradeIds = await db.LessonsStudents
                .Where(ls => ls.GradeId != null && ls.StudentId == studentId && ls.Lesson.CourseId == courseId)
                .Select(ls => ls.GradeId!.Value)
                .ToListAsync(cancellationToken);

            var combinedGradeIds = submissionGradeIds.Union(classworkGradeIds);

            if (!combinedGradeIds.Any())
                return 0f;

            var weightedGrades = await db.Grades
                .Where(g => combinedGradeIds.Contains(g.Id))
                .Select(g => new { g.Value, g.Weight })
                .ToListAsync(cancellationToken);

            var totalWeight = weightedGrades.Sum(g => g.Weight);

            return weightedGrades.Sum(g => g.Value * g.Weight) / totalWeight;
        }

        private static IQueryable<CourseToStudent> BuildFilteredQuery(Request request, int studentId, TimetileDbContext db)
        {
            var baseQuery = db.CoursesStudents
                .AsNoTracking()
                .Where(c => c.StudentId == studentId);

            if (request.TermIds is not null && request.TermIds.Any())
                baseQuery = baseQuery.Where(cs =>
                    request.TermIds.Contains(cs.Course.TermId)
                );

            return baseQuery;
        }

        public sealed record Request(
            int Id,
            int[]? TermIds,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int CourseId,
            CourseInfo Course,
            int? ExamGradeId,
            bool HasExam,
            short PositionX,
            short PositionY,
            float AverageGrade
        );

        private sealed record CourseInfo(
            int Id,
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId,
            string IconUrl
        );
    }
}
