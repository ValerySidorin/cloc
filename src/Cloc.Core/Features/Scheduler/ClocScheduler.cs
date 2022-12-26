using Cloc.Core.Extensions;
using System.Diagnostics;

namespace Cloc;

public class ClocScheduler : IDisposable
{
    protected ClocOptions Options { get; }

    protected IClocJobExecutor Executor { get; set; }

    protected ICollection<PeriodicTimer> Timers { get; }

    public ClocScheduler(
        ClocOptions options,
        IClocJobExecutor executor = null)
    {
        Debug.Assert(options is not null);
        Options = options;
        Executor = executor ?? new ClocJobExecutor();
        Timers = new List<PeriodicTimer>();
    }

    public void Start(CancellationToken cancellationToken = default)
    {
        if (Options.Jobs is not null && Options.Jobs.Any())
        {
            foreach (var jobOptions in Options.Jobs)
            {
#pragma warning disable CS4014
                ScheduleAsync(jobOptions, cancellationToken);
#pragma warning restore CS4014
            }
        }
    }

    public void Enlist<TClocJob>(TClocJob job = default)
            where TClocJob : class, IClocJob
    {
        Executor.AddJob(job);
    }

    private async Task ScheduleAsync(
        ClocJobOptions options, CancellationToken cancellationToken = default)
    {
        var startAt = options.GetStartingPointOfTime();
        await DelayStartAsync(startAt, cancellationToken).ConfigureAwait(false);

        var interval = options.GetPollInterval();
        var timer = new PeriodicTimer(interval);
        Timers.Add(timer);

        await Executor.ExecuteAsync(options, cancellationToken)
            .ConfigureAwait(false);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false) &&
            !cancellationToken.IsCancellationRequested)
        {
            if (options.DayAt.HasValue && options.DayAt.Value != DateTime.Now.Day)
            {
                return;
            }

            await Executor.ExecuteAsync(options, cancellationToken)
            .ConfigureAwait(false);
        }
    }

    private static async Task DelayStartAsync(
        DateTimeOffset startAt, 
        CancellationToken cancellationToken = default)
    {
        if (startAt > DateTimeOffset.Now)
        {
            await Task.Delay(startAt - DateTimeOffset.Now, cancellationToken);
        }
    }

    public void Dispose()
    {
        foreach (var timer in Timers)
        {
            timer.Dispose();
        }
    }
}
