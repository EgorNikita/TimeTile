using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Files.Services.Interfaces;
using TimeTile.API.Users.Services.Interfaces;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Students.Endpoints;

public class CreateStudent : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new Student")
        .WithRequestValidation<Request>();

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
        string Login,
        string Password
    );
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            // Firstname: not empty, max 255, letters + space/comma/dot/quote/hyphen only
            RuleFor(u => u.Firstname)
                .NotEmpty()
                .MaximumLength(255)
                .Matches("^[a-zA-Z ,.'-]+$")
                .WithMessage("Firstname can contain only letters, spaces, commas, periods, apostrophes, and hyphens.");
        
            // Lastname: same as firstname
            RuleFor(u => u.Lastname)
                .NotEmpty()
                .MaximumLength(255)
                .Matches("^[a-zA-Z ,.'-]+$")
                .WithMessage("Lastname can contain only letters, spaces, commas, periods, apostrophes, and hyphens.");
            
            // BirthDate: not in future
            RuleFor(u => u.BirthDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("BirthDate cannot be in the future.");

            // PhoneNumber: must match pattern
            RuleFor(u => u.PhoneNumber)
                .Matches(@"^(\+\d{1,2} )?\(?\d{3}\)?[ .-]\d{3}[ .-]\d{4}$")
                .When(u => !string.IsNullOrEmpty(u.PhoneNumber))
                .WithMessage("PhoneNumber format is invalid.");

            // HomeAddress: max 255, letters, digits, spaces, commas, periods, hyphens, apostrophes
            RuleFor(u => u.HomeAddress)
                .MaximumLength(255)
                .Matches(@"^[A-Za-z\d'.,\- ]*$")
                .When(u => !string.IsNullOrEmpty(u.HomeAddress))
                .WithMessage("HomeAddress contains invalid characters.");
        }
    }

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        Request request, 
        TimetileDbContext database,
        IUserService userService,
        IFileService fileService,
        IPasswordHasher<User> hasher,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var institutionDomain = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(institutionDomain))
            return TypedResults.NotFound();
        
        var institution = await userService.GetInstitutionId(institutionDomain); //Check if found
        
        var trimmedFirstname = request.Firstname.Trim();
        var trimmedLastname = request.Lastname.Trim();
        var password =
            await userService.GenerateDefaultPassword(trimmedFirstname, trimmedLastname, (short)request.BirthDate.Year);

        var avatarPath = await GetAvatarPath(
            request.Avatar,
            trimmedFirstname,
            trimmedLastname,
            userService,
            fileService,
            cancellationToken
        );
        
        var student = new Student
        {
            AvatarPath = avatarPath,
            Firstname = trimmedFirstname,
            Lastname = trimmedLastname,
            HomeAddress = request.HomeAddress.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            BirthDate = DateOnly.FromDateTime(request.BirthDate),
            InstitutionId = institution.Id,
            Login = await userService.GenerateLogin(trimmedFirstname, trimmedLastname, request.BirthDate.Year, institution.Domain),
        };
        
        student.PasswordHash = hasher.HashPassword(student, password);
        
        await database.Students.AddAsync(student, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(
            student.Id,
            student.Firstname,
            student.Lastname,
            student.Login,
            password
        );

        return TypedResults.Ok(response);
    }
    
    private static async Task<string> GetAvatarPath(
        IFormFile? avatar, 
        string firstname, 
        string lastname, 
        IUserService userService,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        await using var avatarStream = avatar != null
            ? avatar.OpenReadStream()
            : await userService.GenerateDefaultAvatar(firstname, lastname);

        var fileName = $"{firstname}_{lastname}_avatar.png";

        var avatarPath = await fileService.SaveFile(
            avatarStream,
            fileName,
            "image/png",
            cancellationToken
        );

        return avatarPath;
    }
    
}