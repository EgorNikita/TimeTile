using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly TimetileDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;
    
    public AuthService(
        TimetileDbContext context,
        IJwtService jwtService,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<AuthResult> Login(
        string login, 
        string password, 
        string? ipAddress = null, 
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(login);
        if (user == null || !ValidatePassword(user, password))
        {
            return new AuthResult(false, Error: "Invalid credentials");
        }
        
        var claims = BuildClaims(user);
        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id, ipAddress);
        
        // Save refresh token to database
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new AuthResult(true, accessToken, refreshToken.Token);
    }
    
    public async Task<AuthResult> RefreshToken(
        string refreshToken, 
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        
        if (token == null || !token.IsActive)
        {
            return new AuthResult(false, Error: "Invalid refresh token");
        }
        
        // Revoke current token and create new one
        MarkRevoked(token, ipAddress);
        
        var user = token.User;
        var claims = BuildClaims(user);
        var newAccessToken = _jwtService.GenerateAccessToken(claims);
        var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id, ipAddress);
        
        token.ReplaceToken = newRefreshToken;
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new AuthResult(true, newAccessToken, newRefreshToken.Token);
    }
    
    public async Task<bool> RevokeToken(
        string refreshToken, 
        string? ipAddress = null, 
        CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        
        if (token == null || !token.IsActive)
            return false;
        
        MarkRevoked(token, ipAddress);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task RevokeAllUserTokens(
        int userId, 
        string? ipAddress = null, 
        CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        foreach (var token in tokens)
        {
            MarkRevoked(token, ipAddress);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<User?> FindUserAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Institution)
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
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
    
    private void MarkRevoked(RefreshToken token, string? ipAddress)
    {
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        _context.RefreshTokens.Update(token);
    }

}