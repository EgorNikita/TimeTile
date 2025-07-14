using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Lessons.Endpoints.Get
{
    public class GetLessonsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of lessons")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var lessons = await BuildFilteredQuery(request, db)
                .Include(l => l.Course)
                .Include(l => l.LessonToTimetableUnits)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.LessonToTimetableUnits.Select(lt => lt.TimetableUnitId).ToArray(),
                    x.CourseId,
                    x.Course.SubjectId,
                    x.Course.TeacherId,
                    x.ClassroomId,
                    x.LessonStatusId,
                    x.Date,
                    x.Description,
                    x.AssignmentId
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(lessons);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Lesson> BuildFilteredQuery(Request request, TimetileDbContext db)
        {
            var baseQuery = db.Lessons
                .AsNoTracking();

            // Date
            if (request.From is not null)
                baseQuery = baseQuery.Where(l =>
                    l.Date >= request.From.Value.ToUniversalTime()
                );

            if (request.Until is not null)
                baseQuery = baseQuery.Where(l =>
                    l.Date <= request.Until.Value.ToUniversalTime()
                );

            // FKs
            if (request.ClassroomIds is not null && request.ClassroomIds.Any())
                baseQuery = baseQuery.Where(l =>
                    request.ClassroomIds.Contains(l.ClassroomId)
                );

            if (request.TimetableUnitIds is not null && request.TimetableUnitIds.Any())
                baseQuery = baseQuery.Where(l =>
                    l.LessonToTimetableUnits.Any(lt => request.TimetableUnitIds.Contains(lt.TimetableUnitId))
                );

            if (request.CourseIds is not null && request.CourseIds.Any())
                baseQuery = baseQuery.Where(l =>
                    request.CourseIds.Contains(l.CourseId)
                );

            if (request.LessonStatusIds is not null && request.LessonStatusIds.Any())
                baseQuery = baseQuery.Where(l =>
                    request.LessonStatusIds.Contains(l.LessonStatusId)
                );

            // Extra associations
            if (request.StudentIds is not null && request.StudentIds.Any())
                baseQuery = baseQuery.Where(l =>
                    l.Course.CoursesToStudents.Any(cs =>
                        request.StudentIds.Contains(cs.StudentId)
                    )
                );

            if (request.GroupIds is not null && request.GroupIds.Any())
                baseQuery = baseQuery.Where(l =>
                    l.Course.CoursesToStudents.Any(cs =>
                        cs.Student.GroupId != null &&
                        request.GroupIds.Contains(cs.Student.GroupId.Value)
                    )
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? StudentIds = null,
            int[]? GroupIds = null,
            int[]? ClassroomIds = null,
            int[]? TimetableUnitIds = null,
            int[]? CourseIds = null,
            int[]? LessonStatusIds = null,
            DateTimeOffset? From = null,
            DateTimeOffset? Until = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByStudentsRequest;

        private sealed record Response(
            int Id,
            int[] TimetableUnitIds,
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
