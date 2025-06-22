using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Subjects.Endpoints.GetById
{
    public class GetSubjectByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapGet("/{id}", Handle)
                .WithSummary("Returns Subject by passed Id")
                .WithRequestValidation<Request>();
        }

        private static async Task<Ok<Result<Response>>> Handle(
            [AsParameters] Request request,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            // Find Subject
            var subject = await db.Subjects
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            var response = new Response(
                subject.Id,
                subject.Title
            );

            var result = Result.Success(response);

            return TypedResults.Ok(result);
        }

        public sealed record Request(
            int Id
        );

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
