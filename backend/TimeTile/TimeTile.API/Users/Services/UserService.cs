using System.Globalization;
using System.Security.Cryptography;
using TimeTile.API.Users.Services.Interfaces;

namespace TimeTile.API.Users.Services;

public class UserService : IUserService
{
    private readonly IAvatarService _avatarService;
    
    public UserService(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    public Task<string> GenerateLogin(string firstname, string lastname, int birthYear, string institutionDomain)
    {
        var fn = firstname.Length >= 4 
            ? firstname[..4].ToLower(CultureInfo.InvariantCulture) 
            : firstname.ToLower(CultureInfo.InvariantCulture);

        var ln = lastname.Length >= 5 
            ? lastname[..5].ToLower(CultureInfo.InvariantCulture) 
            : lastname.ToLower(CultureInfo.InvariantCulture);

        var login = $"{fn}.{ln}{birthYear}@{institutionDomain}";
        return Task.FromResult(login);
    }

    public Task<string> GenerateDefaultPassword(string firstname, string lastname, short birthYear)
    {
        var basePart = $"{firstname[..1].ToUpper(CultureInfo.InvariantCulture)}{lastname[..1].ToLower(CultureInfo.InvariantCulture)}{birthYear % 100:D2}";
        var suffix = RandomNumberGenerator.GetInt32(1000, 10000);
        var password = $"{basePart}{suffix}";
        return Task.FromResult(password);
    }

    public Task<Stream> GenerateDefaultAvatar(string firstname, string lastname)
    {
        return _avatarService.GenerateDefaultAvatar(firstname, lastname);
    }
}