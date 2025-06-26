namespace TimeTile.Core.Common.Interfaces.Services;

public interface IPermissionService
{
    Task<bool> UserHasPermissionAsync(int userId, string permission);
    Task<List<string>> GetUserPermissionsAsync(int userId);
}