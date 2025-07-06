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

namespace TimeTile.API.Subjects.Endpoints.Get
{
    public class GetSubjectsEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of subjects")
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
            var subjects = await BuildFilteredQuery(request, institutionId, db)
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

            var result = Result.Success(subjects);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Subject> BuildFilteredQuery(Request request, int institutionId, TimetileDbContext db)
        {
            var baseQuery = db.Subjects
                .AsNoTracking()
                .Where(s => s.InstitutionId == institutionId);

            if (request.TeacherIds is not null && request.TeacherIds.Any())
                baseQuery = baseQuery.Where(s => 
                    s.TeachersToSubject.Any(ts => request.TeacherIds.Contains(ts.TeacherId))
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? TeacherIds = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByTeachersRequest;

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
