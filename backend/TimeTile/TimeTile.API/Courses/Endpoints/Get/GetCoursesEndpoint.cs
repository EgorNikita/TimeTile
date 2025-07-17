using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.Get
{
    public class GetCoursesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of courses")
                .WithRequestValidation<Request>()
                .RequireUserId();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            ICourseService courseService,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            IUserProvider userProvider,
            CancellationToken cancellationToken)
        {
            var institutionId = institutionProvider.GetInstitutionId();
            var userId = userProvider.GetUserId();

            var baseQuery = BuildFilteredQuery(request, institutionId, db);

            baseQuery = ApplySorting(baseQuery, userId, request.SortBy, request.Descending);

            // Form a final paged list
            var courses = await baseQuery
                .Include(c => c.Icon)
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.SubjectId,
                    x.TeacherId,
                    x.IsAdvanced,
                    x.TermId,
                    x.CoursesToUsers.Where(cu => cu.UserId == userId).Select(cu => (int?)cu.OrderNumber).FirstOrDefault(),
                    courseService.GetIconUrl(x)
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(courses);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Course> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.Courses
                .AsNoTracking()
                .Where(c => c.InstitutionId == institutionId);

            if (request.SubjectIds is not null && request.SubjectIds.Any())
                baseQuery = baseQuery.Where(c =>
                    request.SubjectIds.Contains(c.SubjectId)
                );

            if (request.TeacherIds is not null && request.TeacherIds.Any())
                baseQuery = baseQuery.Where(c =>
                    request.TeacherIds.Contains(c.TeacherId)
                );

            if (request.TermIds is not null && request.TermIds.Any())
                baseQuery = baseQuery.Where(c =>
                    request.TermIds.Contains(c.TermId)
                );

            if (request.StudentIds is not null && request.StudentIds.Any())
                baseQuery = baseQuery.Where(c =>
                    c.CoursesToStudents.Any(cs => request.StudentIds.Contains(cs.StudentId))
                );

            if (request.GroupIds is not null && request.GroupIds.Any())
                baseQuery = baseQuery.Where(c =>
                    c.CoursesToStudents.Any(cs =>
                        cs.Student.GroupId != null &&
                        request.GroupIds.Contains(cs.Student.GroupId.Value)
                    )
                );

            return baseQuery;
        }

        private static IQueryable<Course> ApplySorting(IQueryable<Course> query, int userId, string? sortBy, bool descending)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                return query.ApplySorting(
                    sortBy,
                    descending
                );
            }

            return descending
                ? query.OrderByDescending(c =>
                    c.CoursesToUsers
                        .Where(cu => cu.UserId == userId)
                        .Select(cu => (int?)cu.OrderNumber)
                        .FirstOrDefault())
                : query.OrderBy(c =>
                    c.CoursesToUsers
                        .Where(cu => cu.UserId == userId)
                        .Select(cu => (int?)cu.OrderNumber)
                        .FirstOrDefault());
        }

        public sealed record Request(
            int[]? SubjectIds = null,
            int[]? TeacherIds = null,
            int[]? TermIds = null,
            int[]? StudentIds = null,
            int[]? GroupIds = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByStudentsRequest, IFilterByTeachersRequest;

        private sealed record Response(
            int Id,
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId,
            int? OrderNumber,
            string IconUrl
        );
    }
}
