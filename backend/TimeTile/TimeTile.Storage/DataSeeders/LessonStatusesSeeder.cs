using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeTile.Core.Common.Constants;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;

namespace TimeTile.Storage.DataSeeders
{
    public class LessonStatusesSeeder
    {
        public static async Task Seed(
            TimetileDbContext db,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            if (await db.LessonStatuses.CountAsync(cancellationToken) == LessonStatuses.Count)
                return;

            var existingStatuses = db.LessonStatuses
                .Select(s => s.Description)
                .ToHashSet();

            var statusesToAdd = LessonStatuses.All
                .Except(existingStatuses)
                .Select(status => new LessonStatus
                {
                    Description = status
                })
                .ToList();

            await db.LessonStatuses.AddRangeAsync(statusesToAdd, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            logger.Information("Added {Count} new lesson statuses: {LessonStatuses}",
                    statusesToAdd.Count,
                    string.Join(", ", statusesToAdd.Select(s => s.Description)));
        }
    }
}
