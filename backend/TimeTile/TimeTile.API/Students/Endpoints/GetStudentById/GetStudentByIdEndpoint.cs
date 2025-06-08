using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetStudentById;

public class GetStudentByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns Student by passed Id")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        int Id
    );
    
    public sealed record Response(
        int Id,
        string Firstname,
        string Lastname,
        string Login,
        string HomeAddress,
        string PhoneNumber,
        DateTime BirthDate,
        string AvatarUrl
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound<Result>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext context,
        CancellationToken cancellationToken)
    {
        var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);

        if (!institutionResult.IsSuccess)
            return TypedResults.NotFound(Result.Failure(institutionResult.Error));
    }
}