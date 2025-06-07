using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.GetInstitutionById
{
    public class GetInstitutionByIdEndpoint : IEndpoint
    {
        public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
            .MapGet("/{id}", Handle)
            .WithSummary("Returns Institution by passed Id")
            .WithRequestValidation<Request>()
            .Produces<Result<Response>>(StatusCodes.Status200OK)
            .Produces<Result>(StatusCodes.Status404NotFound);

        public sealed record Request(
            int Id
        );

        public sealed record Response(
            int Id,
            string Title,
            string Address,
            string PhoneNumber,
            string Email,
            string Domain
        );

        private static async Task<IResult> Handle(
            [AsParameters] Request request,
            TimetileDbContext context,
            CancellationToken cancellationToken)
        {
            var institution = await context.Institutions
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (institution is null)
            {
                var error = Error.From(
                    $"Institution with id '{request.Id}' does not exist.", 
                    "ENTITY_DOES_NOT_EXIST"
                );

                return Results.NotFound(Result.Failure(error));
            }

            var response = new Response(
                institution.Id,
                institution.Title,
                institution.Address,
                institution.PhoneNumber,
                institution.Email,
                institution.Domain
            );

            var result = Result.Success(response);

            return Results.Ok(result);
        }
    }
}
