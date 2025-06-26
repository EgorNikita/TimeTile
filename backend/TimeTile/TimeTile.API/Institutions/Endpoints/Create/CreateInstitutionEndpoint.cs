using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.Create;

public class CreateInstitutionEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/", Handle)
            .WithSummary("Creates a new Institution")
            .WithRequestValidation<Request>();
    }

    private static async Task<Created<Result<Response>>> Handle(
        Request request,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        var institution = new Institution
        {
            Title = request.Title.Trim(),
            Address = request.Address.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            Domain = request.Domain.Trim()
        };

        await db.Institutions.AddAsync(institution, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var response = new Response(
            institution.Id,
            institution.Title,
            institution.Address,
            institution.PhoneNumber,
            institution.Email,
            institution.Domain
        );

        var result = Result.Success(response);

        return TypedResults.Created($"{API.Endpoints.Routes.Institutions}/{institution.Id}", result);
    }

    public sealed record Request(
        string Title,
        string Address,
        string PhoneNumber,
        string Email,
        string Domain
    );

    private sealed record Response(
        int Id,
        string Title,
        string Address,
        string PhoneNumber,
        string Email,
        string Domain
    );
}