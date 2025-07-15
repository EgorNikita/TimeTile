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

namespace TimeTile.API.Students.Endpoints.GetAttendanceCount
{
    public class GetAttendanceCountEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{Id:int}/attendance-count", Handle)
                .WithSummary("Gets attendance of the Student")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var baseQuery = BuildFilteredQuery(request, request.Id, db);

            var totalLessons = await baseQuery
                .CountAsync(cancellationToken);

            var attendedLessons = await baseQuery
                .Where(ls => ls.CameAt != null)
                .CountAsync(cancellationToken);

            var response = new Response(
                totalLessons,
                attendedLessons
            );

            var result = Result.Success(response);

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
            int[]? CourseIds
        );

        private sealed record Response(
            int TotalLessons,
            int AttendedLessons
        );
    }
}
