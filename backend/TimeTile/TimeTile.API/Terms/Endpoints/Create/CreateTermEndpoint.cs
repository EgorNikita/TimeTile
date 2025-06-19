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

        private static async Task<Results<Created<Result<Response>>, BadRequest<Result>>> Handle(
            [FromBody] Request request,
            TimetileDbContext db,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            // Extract InstitutionId
            var institutionId = httpContext.GetInstitutionId();

            // Check if already exists
            var duplicateCheckResult = await IsTermDuplicate(request, institutionId, db, cancellationToken);

            if (duplicateCheckResult.IsFailure)
                return TypedResults.BadRequest(duplicateCheckResult);

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

            return TypedResults.Created($"/terms/{term.Id}", result);
        }

        private static async Task<Result> IsTermDuplicate(
            Request request,
            int institutionId,
            TimetileDbContext db,
            CancellationToken cancellationToken)
        {
            var baseQuery = db.Terms
                .AsNoTracking()
                .Where(x => x.InstitutionId == institutionId);

            // Check terms_institution_title_deleted_at_key
            var isDuplicateOnTitle = await baseQuery
                .AnyAsync(x => x.Title == request.Title, cancellationToken);

            if (isDuplicateOnTitle)
            {
                var error = Error.From(
                    $"A term with the title '{request.Title}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            // Check terms_institution_start_end_deleted_at_key
            var allTerms = await baseQuery
                .ToListAsync(cancellationToken);

            var requestStartDateUtc = request.StartDate.UtcDateTime.Date;
            var requestEndDateUtc = request.EndDate.UtcDateTime.Date;

            var isDuplicateOnTime = allTerms.
                Any(x => 
                    x.StartDate.UtcDateTime.Date == requestStartDateUtc &&
                    x.EndDate.UtcDateTime.Date == requestEndDateUtc);

            if (isDuplicateOnTime)
            {
                var error = Error.From(
                    $"A term with the dates '{requestStartDateUtc}' - '{requestEndDateUtc}' already exists in your institution.",
                    "ENTITY_ALREADY_EXISTS"
                );
                return Result.Failure(error);
            }

            return Result.Success();
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
