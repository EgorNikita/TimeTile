using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication.Services;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Authentication.Endpoints;

public class Login : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithSummary("Logs in a user with roles and InstitutionId")
        .WithRequestValidation<Request>();
    
    public record Request(string Login, string Password);
    public record Response(string Token);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
    
    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        Request request, 
        TimetileDbContext database, 
        Jwt jwt,
        IPasswordHasher<User> hasher,
        CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(request.Login, database, cancellationToken);
        if (user is null || !ValidatePassword(user, request.Password, hasher))
            return TypedResults.Unauthorized();

        var claims = BuildClaims(user);
        var token = jwt.GenerateToken(claims);
    
        return TypedResults.Ok(new Response(token));
    }
    
    private static async Task<User?> FindUserAsync(
        string login, 
        TimetileDbContext database, 
        CancellationToken cancellationToken)
    {
        return await database.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .ThenInclude(r => r.RoleToPermissions)
            .ThenInclude(rtp => rtp.Permission)
            .SingleOrDefaultAsync(u => u.Login == login, cancellationToken);
    }
    
    private static bool ValidatePassword(
        User user, 
        string password, 
        IPasswordHasher<User> hasher)
    {
        return hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success;
    }
    
    private static List<Claim> BuildClaims(User user)
    {
        const string studentRole = "Student";
        const string memberRole = "InstitutionMember";

        var effectiveRole = user.Role.Title == studentRole ? studentRole : memberRole;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Login),
            new(ClaimTypes.Role, effectiveRole),
            new("institution_id", user.InstitutionId.ToString())
        };

        claims.AddRange(user.Role.Permissions.Select(p => new Claim("permission", p.Description)));
    
        return claims;
    }
}