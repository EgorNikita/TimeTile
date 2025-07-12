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

namespace TimeTile.API.Terms.Endpoints.Get
{
    public class GetTermsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of terms")
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
            var terms = await BuildFilteredQuery(request, institutionId, db)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.StartDate,
                    x.EndDate
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(terms);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Term> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.Terms
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId);

            if (request.StartDateFrom is not null)
                baseQuery = baseQuery.Where(t => t.StartDate >= request.StartDateFrom.Value.ToUniversalTime());

            if (request.StartDateUntil is not null)
                baseQuery = baseQuery.Where(t => t.StartDate <= request.StartDateUntil.Value.ToUniversalTime());

            if (request.StudentIds is not null && request.StudentIds.Any())
                baseQuery = baseQuery.Where(t =>
                    t.Courses.SelectMany(c => c.CoursesToStudents)
                        .Any(cs => request.StudentIds.Contains(cs.StudentId))
                );

            return baseQuery;
        }

        public sealed record Request(
            DateTimeOffset? StartDateFrom,
            DateTimeOffset? StartDateUntil,
            int[]? StudentIds,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest;

        private sealed record Response(
            int Id,
            string Title,
            DateTimeOffset StartDate,
            DateTimeOffset EndDate
        );
    }
}
