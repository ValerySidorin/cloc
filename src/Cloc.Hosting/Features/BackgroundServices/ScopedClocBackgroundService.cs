using Microsoft.Extensions.Hosting;

namespace Cloc.Hosting;

internal class ScopedClocBackgroundService : BackgroundService
{
    private readonly ClocSchedulerBase _scheduler;

    public ScopedClocBackgroundService(ClocSchedulerBase scheduler)
    {
        _scheduler = scheduler;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _scheduler.Start(stoppingToken);
        return Task.CompletedTask;
    }
}
