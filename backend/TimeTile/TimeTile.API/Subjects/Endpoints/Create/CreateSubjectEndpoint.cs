using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Subjects.Endpoints.Create
{
    public class CreateSubjectEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new Subject")
                .WithRequestValidation<Request>();
        }

        private static async Task<Created<Result<Response>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            IInstitutionProvider institutionProvider,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = institutionProvider.GetInstitutionId();

            // Save Subject
            var subject = new Subject
            {
                InstitutionId = institutionId,
                Title = request.Title.Trim()
            };

            await db.Subjects.AddAsync(subject, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                subject.Id,
                subject.Title
            );

            var result = Result.Success(response);

            return TypedResults.Created($"/subjects/{subject.Id}", result);
        }

        public sealed record Request(
            string Title
        );

        private sealed record Response(
            int Id,
            string Title
        );
    }
}
