using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.Storage.DataSeeders
{
    public static class PermissionsSeeder
    {
        public static async Task Seed(
            TimetileDbContext db, 
            ILogger logger, 
            CancellationToken cancellationToken = default)
        {
            if (await db.Permissions.CountAsync(cancellationToken) == Permissions.Count)
                return;

            var existingPermissions = db.Permissions
                .Select(p => p.Description)
                .ToHashSet();

            var permissionsToAdd = Permissions.All
                .Except(existingPermissions)
                .Select(permission => new Permission 
                { 
                    Description = permission 
                })
                .ToList();

            if (permissionsToAdd.Any())
            {
                await db.Permissions.AddRangeAsync(permissionsToAdd, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                logger.Information("Added {Count} new permissions: {Permissions}",
                    permissionsToAdd.Count,
                    string.Join(", ", permissionsToAdd.Select(p => p.Description)));
            }
        }
    }
}
