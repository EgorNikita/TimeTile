using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Classrooms.Endpoints.GetBulk
{
    public class GetClassroomsBulkEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/bulk", Handle)
                .WithSummary("Returns classrooms by passed ids")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<List<Response>>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Form a final paged list
            var classrooms = await db.Classrooms
                .AsNoTracking()
                .Where(c => request.Ids.Contains(c.Id))
                .Select(c => new Response(
                    c.Id,
                    c.Title,
                    c.Capacity,
                    c.ClassroomTypeId
                ))
                .ToListAsync(cancellationToken);

            var result = Result.Success(classrooms);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int[] Ids
        );

        public sealed record Response(
            int Id,
            string Title,
            int Capacity,
            int ClassroomTypeId
        );
    }
}
