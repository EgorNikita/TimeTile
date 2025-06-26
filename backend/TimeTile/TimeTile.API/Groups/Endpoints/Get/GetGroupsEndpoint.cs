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

namespace TimeTile.API.Groups.Endpoints.Get
{
    public class GetGroupsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of groups")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            var institutionId = institutionProvider.GetInstitutionId();

            // Form a final paged list
            var groups = await BuildFilteredQuery(request, institutionId, db)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.Title
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(groups);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Group> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.Groups
                .AsNoTracking()
                .Where(g => g.InstitutionId == institutionId);

            if (request.InstitutionMemberIds is not null && request.InstitutionMemberIds.Any())
                baseQuery = baseQuery.Where(g =>
                    g.InstitutionMembersToGroup.Any(mg => request.InstitutionMemberIds.Contains(mg.InstitutionMemberId))
                );

            if (request.CourseIds is not null && request.CourseIds.Any())
                baseQuery = baseQuery.Where(g =>
                    g.Students.Any(s => s.CoursesToStudents.Any(cs => request.CourseIds.Contains(cs.CourseId)))
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? InstitutionMemberIds = null,
            int[]? CourseIds = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
