using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Files.Services.Interfaces;
using TimeTile.API.Users.Services.Interfaces;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints.CreateStudent;

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

    private static async Task<Results<Created<Result<Response>>, NotFound<Result>, BadRequest<Result>>> Handle(
        [FromForm] Request request,
        TimetileDbContext db,
        IUserService userService,
        IFileService fileService,
        IPasswordHasher<User> hasher,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var duplicateCheckResult = await IsStudentDuplicate(
            request, db, cancellationToken);

        if (duplicateCheckResult.IsFailure)
            return TypedResults.BadRequest(duplicateCheckResult);


        var institutionResult = await claimsPrincipal.GetValidatedInstitutionIdAsync(db, cancellationToken);
        if (institutionResult.IsFailure)
            return TypedResults.NotFound(Result.Failure(institutionResult.Error));

        var institutionId = institutionResult.Data;
        var institution = await db.Institutions
            .AsNoTracking()
            .FirstAsync(i => i.Id == institutionId, cancellationToken);

        var firstName = request.Firstname.Trim();
        var lastName = request.Lastname.Trim();
        var birthYear = (short)request.BirthDate.Year;

        var password = await userService.GenerateDefaultPassword(firstName, lastName, birthYear);
        var login = await userService.GenerateUniqueLoginAsync(firstName, lastName, birthYear, institution.Domain);

        var roleResult = await GetStudentRole(db, cancellationToken);
        if (roleResult.IsFailure)
            return TypedResults.NotFound(Result.Failure(roleResult.Error));

        var studentRoleId = roleResult.Data!.Id;
        
        var avatarPath = await GetAvatarPath(
            request.Avatar,
            firstName,
            lastName,
            request.BirthDate,
            userService,
            fileService,
            cancellationToken
        );

        var student = new Student
        {
            Firstname = firstName,
            Lastname = lastName,
            HomeAddress = request.HomeAddress.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            BirthDate = DateOnly.FromDateTime(request.BirthDate),
            AvatarPath = avatarPath,
            Login = login,
            InstitutionId = institution.Id,
            RoleId = studentRoleId
        };

        student.PasswordHash = hasher.HashPassword(student, password);

        try
        {
            await db.Students.AddAsync(student, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            var error = Error.From(e.Message);
            return TypedResults.BadRequest(Result.Failure(error));
        }

        var response = new Response(
            student.Id,
            student.Firstname,
            student.Lastname,
            student.Login
        );

        var result = Result.Success(response);

        return TypedResults.Created($"/students/{student.Id}", result);
    }

    private static async Task<string> GetAvatarPath(
        IFormFile? avatar,
        string firstname,
        string lastname,
        DateTime birthday,
        IUserService userService,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        await using var avatarStream = avatar != null
            ? avatar.OpenReadStream()
            : await userService.GenerateDefaultAvatar(firstname, lastname);

        var fileName = $"{firstname}_{lastname}_{birthday}_avatar.png";

        var avatarPath = await fileService.SaveFile(
            avatarStream,
            fileName,
            cancellationToken
        );

        return avatarPath;
    }

    private static async Task<Result> IsStudentDuplicate(
        Request request,
        TimetileDbContext db,
        CancellationToken cancellationToken)
    {
        var existingStudent = await db.Students
            .AsNoTracking()
            .AnyAsync(s => s.DeletedAt == null &&
                           s.Firstname == request.Firstname &&
                           s.Lastname == request.Lastname &&
                           s.BirthDate == DateOnly.FromDateTime(request.BirthDate),
                cancellationToken);

        if (existingStudent)
        {
            var error = Error.From(
                $"A student with the name '{request.Firstname} {request.Lastname}' and birth date '{request.BirthDate:yyyy-MM-dd}' already exists.",
                "ENTITY_ALREADY_EXISTS"
            );
            return Result.Failure(error);
        }

        return Result.Success();
    }

    private static async Task<Result<Role>> GetStudentRole(TimetileDbContext db, CancellationToken cancellationToken)
    {
        var studentRole = await db.Roles.FirstOrDefaultAsync(r => r.Title == "Student", cancellationToken);
        if (studentRole != null) return Result.Success(studentRole);

        var error = Error.From(
            "The 'Student' role does not exist. Please create it before adding a student.",
            "ROLE_NOT_FOUND"
        );
        return Result.Failure<Role>(error);
    }

    public record Request(
        IFormFile? Avatar,
        string Firstname,
        string Lastname,
        string HomeAddress,
        string PhoneNumber,
        DateTime BirthDate
    );

    private record Response(
        int Id,
        string Firstname,
        string Lastname,
        string Login
    );
}