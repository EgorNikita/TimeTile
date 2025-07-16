using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Enums;
using TimeTile.Storage.Contexts;

namespace TimeTile.API.Submissions.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly TimetileDbContext _db;

        public SubmissionService(TimetileDbContext db)
        {
            _db = db;
        }

        public async Task ExpireOverdueSubmissions(CancellationToken cancellationToken = default)
        {
            await _db.Submissions
                .Where(s =>
                    s.Status == SubmissionStatus.NotSubmitted &&
                    s.Assignment.Deadline <= DateTimeOffset.UtcNow
                )
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.Status, SubmissionStatus.Expired), cancellationToken);
        }

        public async Task<DateTimeOffset?> GetNextDeadline(CancellationToken cancellationToken = default)
        {
            var deadlinesFromFuture = await _db.Submissions
                .AsNoTracking()
                .Where(s => s.Assignment.Deadline > DateTimeOffset.UtcNow)
                .Select(s => s.Assignment.Deadline)
                .ToListAsync(cancellationToken);

            if (deadlinesFromFuture.Any())
            {
                return deadlinesFromFuture.Min();
            }

            return null;
        }
    }
}
