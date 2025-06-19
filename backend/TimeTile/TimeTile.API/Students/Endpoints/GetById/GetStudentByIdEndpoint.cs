using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.GetById;

public class GetStudentByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/{id}", Handle)
            .WithSummary("Returns Student by passed Id")
            .WithRequestValidation<Request>();
    }

    private static async Task<Results<Ok<Result<Response>>, NotFound<Result>, JsonHttpResult<Result>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext db,
        IFileService fileService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var institutionId = httpContext.GetInstitutionId();

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

        var avatarUrl = fileService.GetFileUrl(student.Avatar.StoragePath);

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
}