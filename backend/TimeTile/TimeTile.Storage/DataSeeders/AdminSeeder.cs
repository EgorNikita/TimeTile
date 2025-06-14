using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.Storage.DataSeeders;

public static class AdminSeeder
{
    public const string ADMIN_LOGIN = "admin@timetile.dev";

    public static async Task Seed(
        TimetileDbContext db, 
        IPasswordHasher<User> passwordHasher, 
        IUserService userService,
        IFileService fileService,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(u => u.Login == ADMIN_LOGIN, cancellationToken))
            return;

        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Title == RolesSeeder.ADMIN_ROLE_TITLE, cancellationToken)
            ?? throw new InvalidOperationException("Admin role not found.");

        var user = new User
        {
            Login = ADMIN_LOGIN,
            Role = adminRole,
            Firstname = "John",
            Lastname = "Adminovich",
            BirthDate = new DateOnly(2002, 1, 2),
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Admin St, Admin City, Admin Country"
        };

        var avatarStream = await userService.GenerateDefaultAvatar(user.Firstname, user.Lastname);
        var avatarId = await fileService.SaveFile(avatarStream, "avatar/default.png", cancellationToken);

        user.AvatarId = avatarId;

        user.PasswordHash = passwordHasher.HashPassword(user, "admin123!");
        
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.Information("Added admin user: {User}", ADMIN_LOGIN);
    }
}