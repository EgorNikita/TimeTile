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

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        TimetileDbContext db,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        // Find Student
        var student = await db.Students
            .AsNoTracking()
            .FirstAsync(s => s.Id == request.Id, cancellationToken);

        var response = new Response(
            student.Id,
            student.Firstname,
            student.Lastname,
            student.HomeAddress,
            student.PhoneNumber,
            student.BirthDate,
            student.Login,
            student.GroupId,
            fileService.GetFileUrl(student.Avatar.StoragePath)
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
        string HomeAddress,
        string PhoneNumber,
        DateOnly BirthDate,
        string Login,
        int? GroupId,
        string AvatarUrl
    );
}