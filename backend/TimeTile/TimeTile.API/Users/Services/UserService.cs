using System.Globalization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Users.Services.Interfaces;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Services;

public class UserService : IUserService
{
    private readonly IAvatarService _avatarService;
    private readonly TimetileDbContext _db;

    public UserService(TimetileDbContext db, IAvatarService avatarService)
    {
        _db = db;
        _avatarService = avatarService;
    }

    public async Task<string> GenerateUniqueLoginAsync(string firstname, string lastname, int birthYear,
        string institutionDomain)
    {
        var fn = firstname.Length >= 4 ? firstname[..4].ToLowerInvariant() : firstname.ToLowerInvariant();
        var ln = lastname.Length >= 5 ? lastname[..5].ToLowerInvariant() : lastname.ToLowerInvariant();

        var baseLocalPart = $"{fn}.{ln}{birthYear}";
        var domain = institutionDomain.ToLowerInvariant();

        // Query all existing logins starting with baseLocalPart@
        var existingLogins = await _db.Users
            .AsNoTracking()
            .Where(u => u.Login.StartsWith(baseLocalPart) && u.Login.EndsWith("@" + domain))
            .Select(u => u.Login)
            .ToListAsync();

        // Extract numeric suffixes (if any)
        // Example: baseLocalPart@domain (no suffix)
        // Or baseLocalPart1234@domain (with suffix)
        var maxSuffix = -1;
        foreach (var login in existingLogins)
        {
            // Extract part between baseLocalPart and @domain
            var startIndex = baseLocalPart.Length;
            var endIndex = login.IndexOf('@');
            var suffixPart = login.Substring(startIndex, endIndex - startIndex);

            if (string.IsNullOrEmpty(suffixPart))
                maxSuffix = Math.Max(maxSuffix, 0);
            else if (int.TryParse(suffixPart, out var suffixNum))
                if (suffixNum > maxSuffix)
                    maxSuffix = suffixNum;
        }

        // Increment suffix for new login
        var newLoginLocalPart = maxSuffix == -1 ? baseLocalPart : $"{baseLocalPart}{maxSuffix + 1}";

        var newLogin = $"{newLoginLocalPart}@{domain}";

        return newLogin;
    }

    public Task<string> GenerateDefaultPassword(string firstname, string lastname, short birthYear)
    {
        var basePart =
            $"{firstname[..1].ToUpper(CultureInfo.InvariantCulture)}{lastname[..1].ToLower(CultureInfo.InvariantCulture)}{birthYear % 100:D2}";
        var suffix = RandomNumberGenerator.GetInt32(1000, 10000);
        var password = $"{basePart}{suffix}";
        return Task.FromResult(password);
    }

    public Task<Stream> GenerateDefaultAvatar(string firstname, string lastname)
    {
        return _avatarService.GenerateDefaultAvatar(firstname, lastname);
    }
}