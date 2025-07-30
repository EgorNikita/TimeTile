using TimeTile.Core.Common.UnifiedResponse;
using TimeTile.Core.Models;

namespace TimeTile.API.Common.Api.Http;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Result<User> GetUser()
    {
        var context = _httpContextAccessor.HttpContext;
        var user = context?.GetCurrentUser();

        return user == null 
            ? Result.Failure<User>(Error.From("Current user is not set in the HTTP context.", "ANAUTHORIZED")) 
            : Result.Success(user);
    }
    
}