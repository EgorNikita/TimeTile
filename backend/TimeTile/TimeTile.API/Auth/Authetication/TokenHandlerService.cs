using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Auth.Authorization;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Auth.Authetication;

public class TokenHandlerService : ITokenHandlerService
{
    private readonly TimetileDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public TokenHandlerService(
        TimetileDbContext context,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
    }
 
    public async Task<Result<User>> Login(string login, string password, bool rememberMe, string? ipAddress = null)
    {
        var user = await FindUser(login);
        if (user == null || !ValidatePassword(user, password))
        {
            var error = Error.From(
                "Invalid login or password.",
                "INVALID_CREDENTIALS"
            );
            return Result.Failure<User>(error);
        }

        var claims = BuildClaims(user);
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe 
                ? DateTime.UtcNow.AddDays(7) 
                : DateTime.UtcNow.AddHours(2)
        };

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            claimsPrincipal, 
            authProperties);

        return Result.Success(user);
    }
    
    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<Result<User>> GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return Result.Failure<User>(
                Error.From("User is not authenticated.", "USER_NOT_AUTHENTICATED")
            );

        if (!httpContext.User.TryGetUserId(out var userId))
            return Result.Failure<User>(
                Error.From("User ID is missing or invalid in the current user's claims.", "CLAIM_ID_MISSING")
            );

        var user = await _context.Users.Include(u => u.Role)
            .Include(u => u.Institution)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Result.Failure<User>(
                Error.From($"User with ID '{userId}' does not exist.", "ENTITY_DOES_NOT_EXIST")
            );
        
        return Result.Success(user);
    }

    private async Task<User?> FindUser(string login)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Institution)
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    private bool ValidatePassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }

    private static List<Claim> BuildClaims(User user)
    {
        var effectiveRole = user.Role.Title == GeneralRoles.Student
            ? GeneralRoles.Student
            : GeneralRoles.InstitutionMember;

        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Login),
            new(ClaimTypes.Role, effectiveRole),
            new(CustomClaimTypes.InstitutionId, user.InstitutionId?.ToString() ?? string.Empty),
            new(CustomClaimTypes.InstitutionDomain, user.Institution?.Domain ?? string.Empty)
        };
    }
}