using Furion.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application.Job
{
    public class TimedReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

        public TimedReminderBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var count = 0;
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var taskService = scope.ServiceProvider.GetRequiredService<ITimedReminderJob>();
                    $"Job Begin[{++count}]-----------------------".LogInformation<TimedReminderBackgroundService>();
                    await taskService.Do();
                    $"Job End[{count}]-------------------------".LogInformation<TimedReminderBackgroundService>();
                }
                catch (Exception ex)
                {
                    $"Job Error: {ex.Message}".LogError<TimedReminderBackgroundService>();
                }
            }
        }
    }
}
