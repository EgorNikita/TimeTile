using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;

namespace TimeTile.Core.Common.Interfaces.Services;

public interface ITokenHandlerService
{
    Task<Result<User>> Login(string login, string password, bool rememberMe, string? ipAddress = null);
    Task Logout();
    Task<Result<User>> GetCurrentUser();
}