using System.Security.Claims;
using System.Threading;
using Bogus.DataSets;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
using TimeTile.API.Authentication.Endpoints;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Api.Http;
using TimeTile.API.Common.Constants;
using TimeTile.API.Files.Services;
using TimeTile.API.Users.Services;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.DataSeeders;

namespace TimeTile.API.Students.Endpoints.Create;

public class CreateStudentEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/", Handle)
            .WithSummary("Creates a new Student")
            .WithRequestValidation<Request>()
            .DisableAntiforgery();
    }

    private static async Task<Created<Result<Response>>> Handle(
        [FromForm] Request request,
        TimetileDbContext db,
        IUserService userService,
        IInstitutionProvider institutionProvider,
        CancellationToken cancellationToken)
    {
        // Extract institutionId
        var institutionId = institutionProvider.GetInstitutionId();

        // Save student
        var studentRole = await db.Roles
            .AsNoTracking()
            .FirstAsync(r => r.Title == GeneralRoles.Student, cancellationToken);

        var student = await userService.CreateUser<Student>(
            request.Avatar,
            request.Firstname,
            request.Lastname,
            request.HomeAddress,
            request.PhoneNumber,
            request.BirthDate,
            institutionId,
            studentRole.Id,
            s =>
            {
                s.GroupId = request.GroupId;
            },
            cancellationToken
        );

        // Return result
        var response = new Response(
            student.Id,
            student.Firstname,
            student.Lastname,
            student.HomeAddress,
            student.PhoneNumber,
            student.BirthDate,
            student.Login,
            student.GroupId,
            student.Avatar.FileGuid.ToString()
        );

        var result = Result.Success(response);

        return TypedResults.Created($"/students/{student.Id}", result);
    }

    public record Request
    {
        public IFormFile? Avatar { get; init; }
        public string Firstname { get; init; } = null!;
        public string Lastname { get; init; } = null!;
        public string HomeAddress { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
        public DateOnly BirthDate { get; init; }
        public int? GroupId { get; init; }
    };

    private record Response(
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