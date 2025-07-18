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
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.Get
{
    public class GetAssignmentsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                 .MapGet("/", Handle)
                 .WithSummary("Returns a page of assignments")
                 .WithRequestValidation<Request>()
                 .RequireAuthorization(Permissions.Assignments.Get);
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            IInstitutionProvider institutionProvider,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            // Form a final paged list
            var lessons = await BuildFilteredQuery(request, institutionId, db)
                .Include(a => a.Lesson)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.Description,
                    x.PublishedAt,
                    x.Deadline,
                    x.UploadAfterDeadline,
                    x.Lesson.CourseId,
                    db.AssignmentsFiles.Any(f => f.AssignmentId == x.Id)
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(lessons);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Assignment> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.Assignments
                .AsNoTracking()
                .Where(a => a.Lesson.Course.InstitutionId == institutionId);

            if (request.CourseIds is not null && request.CourseIds.Any())
                baseQuery = baseQuery.Where(c =>
                    request.CourseIds.Contains(c.Lesson.CourseId)
                );

            if (request.StudentIds is not null && request.StudentIds.Any())
                baseQuery = baseQuery.Where(c =>
                    c.Lesson.Course.CoursesToStudents.Any(cs => 
                        request.StudentIds.Contains(cs.StudentId)
                    )
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? StudentIds = null,
            int[]? CourseIds = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByStudentsRequest;

        private sealed record Response(
            int Id,
            string Title,
            string Description,
            DateTimeOffset PublishedAt,
            DateTimeOffset Deadline,
            bool UploadAfterDeadline,
            int CourseId,
            bool HasAttachments
        );
    }
}
