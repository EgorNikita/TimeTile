using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.Storage.DataSeeders
{
    public static class RolesSeeder
    {
        private static readonly List<string> _studentPermissions =
        [
            Permissions.GetSchedule,
            Permissions.GetOwnGroup
        ];

        public static async Task SeedRequiredRoles(
            TimetileDbContext db,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            await SeedAdminRole(db, logger, cancellationToken);
            await SeedStudentRole(db, logger, cancellationToken);
        }

        private static async Task SeedAdminRole(
            TimetileDbContext db, 
            ILogger logger, 
            CancellationToken cancellationToken)
        {
            if (await db.Roles.AnyAsync(r => r.Title == GeneralRoles.Admin, cancellationToken))
                return;

            var permission = await db.Permissions.FirstOrDefaultAsync(p => p.Description == Permissions.CreateInstitution, cancellationToken)
                ?? throw new InvalidOperationException($"Required permission '{Permissions.CreateInstitution}' not found.");

            var role = new Role()
            {
                Title = GeneralRoles.Admin,
                RoleToPermissions =
                [
                    new RoleToPermission()
                    {
                        PermissionId = permission.Id
                    }
                ]
            };

            await db.Roles.AddAsync(role, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            logger.Information("Added role: {Role}", role.Title);
        }

        private static async Task SeedStudentRole(
            TimetileDbContext db,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            if (await db.Roles.AnyAsync(r => r.Title == GeneralRoles.Student, cancellationToken))
                return;

            var permissions = await db.Permissions
                .Where(p => _studentPermissions.Contains(p.Description))
                .ToListAsync(cancellationToken);

            if (permissions.Count != _studentPermissions.Count)
                throw new InvalidOperationException($"Required permissions for student are not found.");

            var role = new Role()
            {
                Title = GeneralRoles.Student,
                RoleToPermissions = permissions.Select(p => new RoleToPermission()
                {
                    PermissionId = p.Id
                }).ToList()
            };

            await db.Roles.AddAsync(role, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            logger.Information("Added role: {Role}", role.Title);
        }
    }
}
