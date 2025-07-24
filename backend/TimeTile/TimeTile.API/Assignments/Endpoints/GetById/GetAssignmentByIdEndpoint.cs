using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Pagination.PagedRequest;
using TimeTile.API.Common.Api.Requests;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Assignments.Endpoints.GetById
{
    public class GetAssignmentByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns Assignment by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find Assignment
            var assignment = await db.Assignments
                .AsNoTracking()
                .Include(a => a.Lesson)
                .Include(a => a.AssignmentToFiles)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            var response = new Response(
                assignment.Id,
                assignment.Title,
                assignment.Description,
                assignment.PublishedAt,
                assignment.Deadline,
                assignment.UploadAfterDeadline,
                assignment.Lesson.CourseId,
                assignment.AssignmentToFiles.Any()
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

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
