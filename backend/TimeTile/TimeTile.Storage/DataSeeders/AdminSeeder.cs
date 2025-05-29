using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.Storage.DataSeeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(TimetileDbContext context, IPasswordHasher<User> passwordHasher, ILogger? logger = null)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (passwordHasher == null)
        {
            throw new ArgumentNullException(nameof(passwordHasher));
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await SeedPermissionAsync(context, logger);
            await SeedRoleAsync(context, logger);
            await SeedUserAsync(context, passwordHasher, logger);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            logger?.Information("Admin seeding completed successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger?.Error(ex, "Error occurred during admin seeding.");
            throw;
        }
    }
    
    private static async Task SeedPermissionAsync(TimetileDbContext context, ILogger? logger)
    {
        const string permissionDescription = "CreateInstitution";
        
        if (await context.Permissions.AnyAsync(p => p.Description == permissionDescription))
        {
            logger?.Information("Permission '{Permission}' already exists, skipping.", permissionDescription);
            return;
        }

        context.Permissions.Add(new Permission
        {
            Description = permissionDescription,
            CreatedAt = DateTime.UtcNow
        });

        logger?.Information("Added permission: {Permission}", permissionDescription);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRoleAsync(TimetileDbContext context, ILogger? logger)
    {
        const string roleTitle = "Admin";

        if (await context.Roles.AnyAsync(r => r.Title == roleTitle))
        {
            logger?.Information("Role '{Role}' already exists, skipping.", roleTitle);
            return;
        }

        var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Description == "Create Institution");
        if (permission == null)
        {
            throw new InvalidOperationException("Required permission 'Create Institution' not found.");
        }

        context.Roles.Add(new Role
        {
            Title = roleTitle,
            CreatedAt = DateTime.UtcNow,
            RoleToPermissions = new List<RoleToPermission>
            {
                new RoleToPermission
                {
                    Permission = permission,
                    CreatedAt = DateTime.UtcNow
                }
            }
        });

        logger?.Information("Added role: {Role}", roleTitle);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUserAsync(TimetileDbContext context, IPasswordHasher<User> passwordHasher, ILogger? logger)
    {
        const string adminLogin = "admin@timetile.dev";

        if (await context.Users.AnyAsync(u => u.Login == adminLogin))
        {
            logger?.Information("Admin user '{User}' already exists, skipping.", adminLogin);
            return;
        }

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == "Admin");
        if (adminRole == null)
        {
            throw new InvalidOperationException("Admin role not found.");
        }

        var user = new User
        {
            Login = adminLogin,
            Role = adminRole,
            Firstname = "John",
            Lastname = "Adminovich",
            AvatarPath = "avatar/default.png",
            BirthDate = new DateOnly(2002, 1, 2),
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Admin St, Admin City, Admin Country",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "admin123!");
        
        context.Users.Add(user);
        logger?.Information("Added admin user: {User}", adminLogin);
        await context.SaveChangesAsync();
    }
}