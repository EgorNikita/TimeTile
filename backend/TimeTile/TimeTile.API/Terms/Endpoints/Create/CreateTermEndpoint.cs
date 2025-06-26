using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Terms.Endpoints.Create
{
    public class CreateTermEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app
                .MapPost("/", Handle)
                .WithSummary("Creates a new Term")
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

            // Save term
            var term = new Term
            {
                InstitutionId = institutionId,
                Title = request.Title.Trim(),
                StartDate = request.StartDate.ToUniversalTime(),
                EndDate = request.EndDate.ToUniversalTime()
            };

            await db.Terms.AddAsync(term, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Return result
            var response = new Response(
                term.Id,
                term.Title,
                term.StartDate,
                term.EndDate
            );

            var result = Result.Success(response);

            return TypedResults.Created($"/{API.Endpoints.Routes.Terms}/{term.Id}", result);
        }

        public sealed record Request(
            string Title,
            DateTimeOffset StartDate,
            DateTimeOffset EndDate
        );

        private sealed record Response(
            int Id,
            string Title,
            DateTimeOffset StartDate,
            DateTimeOffset EndDate
        );
    }
}
