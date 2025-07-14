using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetLessons
{
    public class GetStudentLessonsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{Id:int}/lessons", Handle)
                .WithSummary("Gets Lessons of the Student")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var relations = await BuildFilteredQuery(request, request.Id, db)
                .Include(ls => ls.Lesson)
                    .ThenInclude(l => l.Course)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(ls => new Response(
                    ls.LessonId,
                    new LessonInfo(
                        ls.Lesson.Id,
                        ls.Lesson.TimetableUnitId,
                        ls.Lesson.CourseId,
                        ls.Lesson.Course.SubjectId,
                        ls.Lesson.Course.TeacherId,
                        ls.Lesson.ClassroomId,
                        ls.Lesson.LessonStatusId,
                        ls.Lesson.Date,
                        ls.Lesson.Description,
                        ls.Lesson.AssignmentId
                    ),
                    ls.CameAt,
                    ls.LeftAt,
                    ls.GradeId
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(relations);

            return TypedResults.Ok(result);
        }

        private static IQueryable<LessonToStudent> BuildFilteredQuery(Request request, int studentId, TimetileDbContext db)
        {
            var baseQuery = db.LessonsStudents
                .AsNoTracking()
                .Where(ls => ls.StudentId == studentId);

            if (request.CourseIds is not null && request.CourseIds.Any())
                baseQuery = baseQuery.Where(ls =>
                    request.CourseIds.Contains(ls.Lesson.CourseId)
                );

            return baseQuery;
        }

        public sealed record Request(
            int Id,
            int[]? CourseIds,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int LessonId,
            LessonInfo Lesson,
            DateTimeOffset? CameAt,
            DateTimeOffset? LeftAt,
            int? GradeId
        );

        private sealed record LessonInfo(
            int Id,
            int TimetableUnitId,
            int CourseId,
            int SubjectId,
            int TeacherId,
            int ClassroomId,
            int LessonStatusId,
            DateTimeOffset Date,
            string Description,
            int? AssignmentId
        );
    }
}
