using System.Security.Claims;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    RefreshToken GenerateRefreshToken(int userId, string? ipAddress = null);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}