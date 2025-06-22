using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Constants;
using TimeTile.API.Files.Services;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Users.Services;

public class UserService : IUserService
{
    private readonly TimetileDbContext _db;
    private readonly IAvatarService _avatarService;
    private readonly IFileService _fileService;
    private readonly IPasswordHasher<User> _hasher;

    public UserService(TimetileDbContext db, IAvatarService avatarService, IFileService fileService, IPasswordHasher<User> hasher)
    {
        _db = db;
        _avatarService = avatarService;
        _fileService = fileService;
        _hasher = hasher;
    }

    public async Task<T> CreateUser<T>(
        IFormFile? avatar,
        string firstname, 
        string lastname,
        string homeAddress,
        string phoneNumber,
        DateOnly birthDate,
        int institutionId,
        int roleId,
        Action<T> configureSpecificProperties,
        CancellationToken cancellationToken)
        where T : User, new()
    {
        var institution = await _db.Institutions
            .AsNoTracking()
            .FirstAsync(i => i.Id == institutionId, cancellationToken);

        firstname = firstname.Trim();
        lastname = lastname.Trim();
        var birthYear = (short)birthDate.Year;

        var password = await GenerateDefaultPassword(firstname, lastname, birthYear);
        var login = await GenerateUniqueLoginAsync(firstname, lastname, birthYear, institution.Domain);

        var user = new T
        {
            Firstname = firstname,
            Lastname = lastname,
            HomeAddress = homeAddress,
            PhoneNumber = phoneNumber,
            BirthDate = birthDate,
            Login = login,
            InstitutionId = institutionId,
            RoleId = roleId
        };

        user.PasswordHash = _hasher.HashPassword(user, password);

        // Extra fields for children will be initialized here
        configureSpecificProperties(user);

        await SaveUser(user, avatar, cancellationToken);

        return user;
    }

    private async Task SaveUser<T>(
        T user,
        IFormFile? avatar,
        CancellationToken cancellationToken)
        where T : User
    {
        using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var avatarId = await GetAvatarId(
                avatar,
                user.Firstname,
                user.Lastname,
                user.BirthDate,
                cancellationToken
            );

            user.AvatarId = avatarId;

            await _db.Set<T>().AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken); 
        }
        catch
        {
            await _fileService.DeleteFilePhysically(user.AvatarId, cancellationToken);

            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }

    private async Task<int> GetAvatarId(
        IFormFile? avatar,
        string firstname,
        string lastname,
        DateOnly birthday,
        CancellationToken cancellationToken)
    {
        await using var avatarStream = avatar != null
            ? avatar.OpenReadStream()
            : await GenerateDefaultAvatar(firstname, lastname);

        return await SaveAvatar(
            avatarStream,
            firstname,
            lastname,
            birthday,
            cancellationToken
        );
    }

    public async Task<int> SaveAvatar(
            Stream avatarStream,
            string firstname,
            string lastname,
            DateOnly birthday,
            CancellationToken cancellationToken)
    {
        var fileName = $"{firstname}_{lastname}_{birthday}_avatar.png";

        return await _fileService.SaveFile(
            avatarStream,
            fileName,
            cancellationToken
        );
    }

    private async Task<string> GenerateUniqueLoginAsync(string firstname, string lastname, int birthYear,
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

    private Task<string> GenerateDefaultPassword(string firstname, string lastname, short birthYear)
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