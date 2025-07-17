
using TimeTile.Core.Common.Interfaces.Services;

namespace TimeTile.API.Common.Api.BackgroundServices
{
    public class SubmissionExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SubmissionExpirationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var submissionService = scope.ServiceProvider.GetRequiredService<ISubmissionService>();

                await submissionService.ExpireOverdueSubmissions(stoppingToken);

                var nextDeadline = await submissionService.GetNextDeadline(stoppingToken);

                TimeSpan delay = CalculateDelay(nextDeadline);

                await Task.Delay(delay, stoppingToken);
            }
        }

        private TimeSpan CalculateDelay(DateTimeOffset? nextDeadline)
        {
            if (nextDeadline.HasValue)
            {
                var timeUntilNextDeadline = nextDeadline.Value - DateTimeOffset.UtcNow;

                // We want to ensure that we don't wait too long between checks.
                if (timeUntilNextDeadline > TimeSpan.FromHours(6))
                {
                    timeUntilNextDeadline = TimeSpan.FromHours(1);
                }

                return timeUntilNextDeadline;
            }
            else
            {
                return TimeSpan.FromMinutes(10);
            }
        }
    }
}
