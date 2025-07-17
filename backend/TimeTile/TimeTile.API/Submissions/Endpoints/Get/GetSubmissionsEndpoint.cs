using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Endpoints.Get
{
    public class GetSubmissionsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                 .MapGet("/", Handle)
                 .WithSummary("Returns a page of submissions")
                 .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            IInstitutionProvider institutionProvider,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            var baseQuery = BuildFilteredQuery(request, institutionId, db);

            baseQuery = ApplySorting(baseQuery, request.SortBy, request.Descending);

            // Form a final paged list
            var grades = await baseQuery
                .Select(x => new Response(
                    x.Id,
                    x.AssignmentId,
                    x.StudentId,
                    x.GradeId,
                    x.Status.ToString(),
                    x.StudentNote,
                    x.Feedback,
                    x.SubmittedAt,
                    db.SubmissionsFiles.Any(sf => sf.SubmissionId == x.Id)
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(grades);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Submission> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.Submissions
                .AsNoTracking()
                .Where(s => s.Assignment.Lesson.Course.InstitutionId == institutionId);

            if (request.Statuses is not null && request.Statuses.Any())
            {
                var statuses = request.Statuses
                    .Select(t => Enum.Parse<SubmissionStatus>(t, ignoreCase: true))
                    .ToArray();

                baseQuery = baseQuery.Where(s =>
                    statuses.Contains(s.Status)
                );
            }

            if (request.AssignmentIds is not null && request.AssignmentIds.Any())
                baseQuery = baseQuery.Where(s => 
                    request.AssignmentIds.Contains(s.AssignmentId)
                );

            if (request.StudentIds is not null && request.StudentIds.Any())
                baseQuery = baseQuery.Where(s =>
                    request.StudentIds.Contains(s.StudentId)
                );

            return baseQuery;
        }

        private static IQueryable<Submission> ApplySorting(IQueryable<Submission> query, string? sortBy, bool descending)
        {
            if (!sortBy.IsNullOrEmpty())
            {
                return query.ApplySorting(
                    sortBy,
                    descending
                );
            }
           
            return descending
                ? query.OrderByDescending(s =>
                    s.Assignment.Deadline)
                : query.OrderBy(s =>
                    s.Assignment.Deadline);
        }

        public sealed record Request(
            int[]? StudentIds = null,
            int[]? AssignmentIds = null,
            string[]? Statuses = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByStudentsRequest;

        private sealed record Response(
            int Id,
            int AssignmentId,
            int StudentId,
            int? GradeId,
            string Status,
            string? StudentNote,
            string? Feedback,
            DateTimeOffset? SubmittedAt,
            bool HasAttachments
        );
    }
}
