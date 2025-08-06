using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Messages.Endpoints.Get
{
    public class GetMessagesEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/", Handle)
                .WithSummary("Returns a page of messages")
                .WithRequestValidation<Request>()
                .RequireAuthorization(Permissions.Messages.Get);
        }

        private static async Task<Ok<Result<PagedList<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var lessons = await BuildFilteredQuery(request, db)
                .ApplySorting(
                    request.SortBy,
                    request.Descending
                )
                .Select(x => new Response
                (
                    x.Id,
                    x.UserId,
                    x.CourseId,
                    x.Content,
                    x.SentAt,
                    x.EditedAt,
                    db.MessagesFiles
                        .Where(mf => mf.MessageId == x.Id)
                        .Select(mf => mf.File.FileGuid.ToString())
                        .ToArray()
                ))
                .ToPagedListAsync(request, cancellationToken);

            var result = Result.Success(lessons);

            return TypedResults.Ok(result);
        }

        private static IQueryable<Message> BuildFilteredQuery(Request request, TimetileDbContext db)
        {
            var baseQuery = db.Messages
                .AsNoTracking();

            // Date
            if (request.From is not null)
                baseQuery = baseQuery.Where(m =>
                    m.SentAt >= request.From.Value.ToUniversalTime()
                );

            if (request.Until is not null)
                baseQuery = baseQuery.Where(m =>
                    m.SentAt <= request.Until.Value.ToUniversalTime()
                );

            // FKs
            if (request.SenderIds is not null && request.SenderIds.Any())
                baseQuery = baseQuery.Where(m =>
                    request.SenderIds.Contains(m.UserId)
                );

            if (request.UserIds is not null && request.UserIds.Any())
                baseQuery = baseQuery.Where(m =>
                    request.UserIds.Contains(m.Course.TeacherId) ||
                    m.Course.CoursesToStudents
                        .Any(cs => request.UserIds.Contains(cs.StudentId))
                );

            if (request.CourseIds is not null && request.CourseIds.Any())
                baseQuery = baseQuery.Where(m =>
                    request.CourseIds.Contains(m.CourseId)
                );

            return baseQuery;
        }

        public sealed record Request(
            int[]? SenderIds = null,
            int[]? UserIds = null,
            int[]? CourseIds = null,
            DateTimeOffset? From = null,
            DateTimeOffset? Until = null,
            int? Page = 1,
            int? PageSize = 10,
            string? SortBy = null,
            bool Descending = false
        ) : IPagedRequest, ISortRequest, IFilterByUsersRequest;

        private sealed record Response(
            int Id,
            int UserId,
            int CourseId,
            string? Content,
            DateTimeOffset SentAt,
            DateTimeOffset? EditedAt,
            string[] FileUrls
        );
    }
}
