using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Cloc.Hosting;

internal class ClocBackgroundService : BackgroundService
{
    private readonly ClocScheduler _scheduler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ClocJobOptions> _jobOptions;

    public ClocBackgroundService(
        ClocScheduler scheduler, 
        IEnumerable<ClocJobOptions> jobOptions,
        IServiceProvider serviceProvider)
    {
        Debug.Assert(scheduler is not null);
        Debug.Assert(jobOptions is not null);
        Debug.Assert(serviceProvider is not null);

        _scheduler = scheduler;
        _jobOptions = jobOptions;
        _serviceProvider = serviceProvider;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var singletonJobs = _serviceProvider.GetServices<ClocJob>();
        foreach (var job in singletonJobs)
        {
            var options = _jobOptions.FirstOrDefault(j => j.Id == job.Id);
            if (options is not null)
            {
                _scheduler.Enlist(new ClocJobExecutor(job, options));
            }
        }

        using var scope = _serviceProvider.CreateScope();
        var scopedJobs = scope.ServiceProvider.GetServices<ScopedClocJob>();
        foreach (var job in scopedJobs)
        {
            var options = _jobOptions.FirstOrDefault(j => j.Id == job.Id);
            if (options is not null)
            {
                _scheduler.Enlist(new ScopedClocJobExecutor(_serviceProvider, options));
            }
        }
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _scheduler.Start(stoppingToken);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _scheduler.Stop();
        return base.StopAsync(cancellationToken);
    }
}
