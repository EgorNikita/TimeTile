using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Files.Services;
using TimeTile.API.Files.Services.Interfaces;
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
        DateOnly BirthDate,
        string AvatarUrl
    );

    private static async Task<Results<Ok<Result<Response>>, NotFound<Result>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext db,
        IFileService fileService,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);
        if (!institutionResult.IsSuccess)
            return TypedResults.NotFound(Result.Failure(institutionResult.Error));
        
        var institutionId = institutionResult.Data;
        
        var student = await db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => 
                s.Id == request.Id && s.InstitutionId == institutionId, cancellationToken
                );
        
        if (student is null)
        {
            var error = Error.From(
                $"Institution with id '{request.Id}' does not exist.", 
                "ENTITY_DOES_NOT_EXIST"
            );

            return TypedResults.NotFound(Result.Failure(error));
        }

        var avatarUrl = await fileService.GetFileUrl(student.AvatarPath, cancellationToken);
        
        var response = new Response(
            student.Id,
            student.Firstname,
            student.Lastname,
            student.Login,
            student.HomeAddress,
            student.PhoneNumber,
            student.BirthDate,
            avatarUrl
        );
            
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}