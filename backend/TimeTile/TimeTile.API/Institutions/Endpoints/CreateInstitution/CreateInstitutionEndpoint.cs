using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Institutions.Endpoints.CreateInstitution;

public class CreateInstitutionEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new Institution")
        .WithRequestValidation<Request>();

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

    private static async Task<Results<Created<Result<Response>>, BadRequest<Result>>> Handle(
        Request request, 
        TimetileDbContext database,
        CancellationToken cancellationToken)
    {
        var duplicateCheckResult = await IsInstitutionDuplicate(
            request.Title, request.Email, request.Domain, database, cancellationToken);

        if (duplicateCheckResult.IsFailure)
            return TypedResults.BadRequest(duplicateCheckResult);

        var institution = new Institution
        {
            Title = request.Title,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Domain = request.Domain
        };

        try
        {
            database.Institutions.Add(institution);
            await database.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            var error = Error.From(e.Message);

            return TypedResults.BadRequest(Result.Failure(error));
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
        
        return TypedResults.Created($"/institutions/{institution.Id}", result);
    }
    
    private static async Task<Result> IsInstitutionDuplicate(
        string title,
        string email,
        string domain,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        var existing = await db.Institutions
            .Where(i => i.Title == title || i.Email == email || i.Domain == domain)
            .Select(i => new { i.Title, i.Email, i.Domain })
            .FirstOrDefaultAsync(cancellationToken);

        if (existing == null)
            return Result.Success();

        if (existing.Title == title)
            return Result.Failure(Error.From($"Institution with name '{title}' already exists.", "ENTITY_EXISTS"));

        if (existing.Email == email)
            return Result.Failure(Error.From($"Institution with email '{email}' already exists.", "ENTITY_EXISTS"));

        if (existing.Domain == domain)
            return Result.Failure(Error.From($"Institution with domain '{domain}' already exists.", "ENTITY_EXISTS"));

        return Result.Failure(Error.From("Institution already exists.", "ENTITY_EXISTS"));
    }
    
}