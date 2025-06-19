using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication;
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

    private static async Task<Results<Created<Result<Response>>, NotFound<Result>, BadRequest<Result>, JsonHttpResult<Result>>> Handle(
        [FromForm] Request request,
        TimetileDbContext db,
        IUserService userService,
        IFileService fileService,
        IPasswordHasher<User> hasher,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        // Extract institutionId
        var institutionId = httpContext.GetInstitutionId();

        // Check if already exists in db
        var duplicateCheckResult = await IsStudentDuplicate(            // TODO: will return false, but login will be already taken
            request, db, cancellationToken);

        if (duplicateCheckResult.IsFailure)
            return TypedResults.BadRequest(duplicateCheckResult);

        // Save student
        var institution = await db.Institutions
            .AsNoTracking()
            .FirstAsync(i => i.Id == institutionId, cancellationToken);

        var firstName = request.Firstname.Trim();
        var lastName = request.Lastname.Trim();
        var birthYear = (short)request.BirthDate.Year;

        var password = await userService.GenerateDefaultPassword(firstName, lastName, birthYear);
        var login = await userService.GenerateUniqueLoginAsync(firstName, lastName, birthYear, institution.Domain);

        var studentRole = await GetStudentRole(db, cancellationToken);

        var student = new Student
        {
            Firstname = firstName,
            Lastname = lastName,
            HomeAddress = request.HomeAddress.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            BirthDate = DateOnly.FromDateTime(request.BirthDate),
            Login = login,
            InstitutionId = institution.Id,
            RoleId = studentRole.Id
        };

        student.PasswordHash = hasher.HashPassword(student, password);

        await SaveStudent(
            student,
            request,
            db,
            userService,
            fileService,
            cancellationToken
        );

        // Return result
        var response = new Response(
            student.Id,
            student.Firstname,
            student.Lastname,
            student.Login
        );

        var result = Result.Success(response);

        return TypedResults.Created($"/students/{student.Id}", result);
    }

    private static async Task SaveStudent(
        Student student,
        Request request,
        TimetileDbContext db,
        IUserService userService,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var avatarId = await GetAvatarId(
                request.Avatar,
                student.Firstname,
                student.Lastname,
                request.BirthDate,
                userService,
                fileService,
                cancellationToken
            );

            student.AvatarId = avatarId;

            await db.Students.AddAsync(student, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await fileService.DeleteFilePhysically(student.AvatarId, cancellationToken);
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }

    private static async Task<int> GetAvatarId(
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

        var avatarId = await fileService.SaveFile(
            avatarStream,
            fileName,
            cancellationToken
        );

        return avatarId;
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

    private static async Task<Role> GetStudentRole(TimetileDbContext db, CancellationToken cancellationToken)
    {
        return await db.Roles.FirstAsync(r => r.Title == GeneralRoles.Student, cancellationToken);
    }

    public record Request
    {
        public IFormFile? Avatar { get; init; }
        public string Firstname { get; init; } = null!;
        public string Lastname { get; init; } = null!;
        public string HomeAddress { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
        public DateTime BirthDate { get; init; }
    };

    private record Response(
        int Id,
        string Firstname,
        string Lastname,
        string Login
    );
}