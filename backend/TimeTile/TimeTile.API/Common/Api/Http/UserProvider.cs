using TimeTile.Core.Models;

namespace TimeTile.API.Common.Api.Http;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User GetUser()
    {
        var context = _httpContextAccessor.HttpContext;

        return context!.GetCurrentUser()
               ?? throw new InvalidOperationException("Current user is not set in the HTTP context.");
    }
    
}