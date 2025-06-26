namespace TimeTile.API.Common.Api.Http;

public interface IUserProvider
{
    int GetUserId();
    string? GetUserIp();
}