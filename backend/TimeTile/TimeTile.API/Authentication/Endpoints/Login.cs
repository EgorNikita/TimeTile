using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Authentication.Services;
using TimeTile.API.Common.Api;
using TimeTile.API.Common.Api.Extensions;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Authentication.Endpoints;

public class Login : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithSummary("Authenticates a user and returns a JWT token with roles and institution claims")
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
            .Where(u => u.Login == login)
            .FirstOrDefaultAsync(cancellationToken); 
    }

    private static bool ValidatePassword(
        User user,
        string password,
        IPasswordHasher<User> hasher)
    {
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }

    private static List<Claim> BuildClaims(User user)
    {
        var effectiveRole = user.Role.Title == GeneralRoles.Student ? GeneralRoles.Student : GeneralRoles.InstitutionMember;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Login),
            new(ClaimTypes.Role, effectiveRole),
            new(CustomClaimTypes.InstitutionId, user.InstitutionId.ToString() ?? string.Empty)
        };

        claims.AddRange(user.Role.Permissions
            .Where(p => !string.IsNullOrEmpty(p.Description))
            .Select(p => new Claim(CustomClaimTypes.Permission, p.Description)));

        return claims;
    }
}