namespace TimeTile.API.Common.Api.Http;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        var context = _httpContextAccessor.HttpContext;

        return context!.GetUserId();
    }

    public string? GetUserIp()
    {
        var context = _httpContextAccessor.HttpContext;

        return context!.GetUserIp();
    }
}