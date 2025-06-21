using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.GetById;

public class GetInstitutionByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/{id}", Handle)
            .WithSummary("Returns Institution by passed Id")
            .WithRequestValidation<Request>();
    }

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        var institution = await db.Institutions
            .AsNoTracking()
            .FirstAsync(i => i.Id == request.Id, cancellationToken);

        var response = new Response(
            institution.Id,
            institution.Title,
            institution.Address,
            institution.PhoneNumber,
            institution.Email,
            institution.Domain
        );

        var result = Result.Success(response);

        return TypedResults.Ok(result);
    }

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
}