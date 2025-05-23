using System.Globalization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Services;

public class UserService : IUserService
{
    private readonly TimetileDbContext _dbContext;
    
    public UserService(TimetileDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<Institution> GetInstitutionId(string institutionDomain)
    {
        _dbContext.Institutions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Domain == institutionDomain)
            .ContinueWith(t =>
            {
                if (t.Result == null)
                {
                    throw new Exception($"Institution with domain {institutionDomain} not found.");
                }
                return t.Result;
            });
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

    public Task<string> GenerateDefaultAvatar(string firstname, string lastname)
    {
        var initials = $"{char.ToUpper(firstname[0], CultureInfo.InvariantCulture)}{char.ToUpper(lastname[0], CultureInfo.InvariantCulture)}";
        return Task.FromResult($"/avatars/{initials}.png");
    }
}