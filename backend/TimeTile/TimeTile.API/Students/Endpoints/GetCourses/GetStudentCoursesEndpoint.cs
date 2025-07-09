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
                .Select(cs => new Response(
                    cs.CourseId,
                    new CourseInfo(
                        cs.Course.Id,
                        cs.Course.Title,
                        cs.Course.SubjectId,
                        cs.Course.TeacherId,
                        cs.Course.IsAdvanced,
                        cs.Course.TermId,
                        courseService.GetIconUrl(cs.Course)
                    ),
                    cs.ExamGradeId,
                    cs.HasExam,
                    cs.PositionX,
                    cs.PositionY
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(relations);

            return TypedResults.Ok(result);
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
            short PositionY
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
