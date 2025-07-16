using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Courses.Services;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Courses.Endpoints.GetBulk
{
    public class GetCoursesBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns courses by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            ICourseService courseService,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var courses = await db.Courses
                .AsNoTracking()
                .Where(c => request.Ids.Contains(c.Id))
                .Include(c => c.Icon)
                .Select(x => new Response
                (
                    x.Id,
                    x.Title,
                    x.SubjectId,
                    x.TeacherId,
                    x.IsAdvanced,
                    x.TermId,
                    courseService.GetIconUrl(x)
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(courses);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

        private sealed record Response(
            int Id,
            string Title,
            int SubjectId,
            int TeacherId,
            bool IsAdvanced,
            int TermId,
            string IconUrl
        );
    }
}
