namespace TimeTile.Core.Common.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> Login(string login, string password, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<AuthResult> RefreshToken(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<bool> RevokeToken(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokens(int userId, string? ipAddress = null, CancellationToken cancellationToken = default);
}

public record AuthResult(bool Success, string? AccessToken = null, string? RefreshToken = null, string? Error = null);