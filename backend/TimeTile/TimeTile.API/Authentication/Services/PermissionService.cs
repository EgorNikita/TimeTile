using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Authentication.Services;

public class PermissionService : IPermissionService
{
    private readonly TimetileDbContext _context;

    public PermissionService(TimetileDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string permission)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Role.RoleToPermissions)
            .Select(rtp => rtp.Permission.Description)
            .AnyAsync(p => p == permission);
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Role.RoleToPermissions)
            .Select(rtp => rtp.Permission.Description)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToListAsync();
    }
}