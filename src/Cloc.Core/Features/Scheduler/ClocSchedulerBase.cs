using EnsureThat;
using System.Timers;

namespace Cloc;

public abstract class ClocSchedulerBase : IDisposable
{
    protected ClocOptions Options { get; }

    protected ICollection<PeriodicTimer> Timers { get; }

    protected ICollection<Task> RunningTasks { get; }

    public ClocSchedulerBase(ClocOptions options)
    {
        EnsureArg.IsNotNull(options, nameof(options));
        Options = options;
        Timers = new List<PeriodicTimer>();
        RunningTasks = new List<Task>();
    }

    public void Start(CancellationToken cancellationToken = default)
    {
        if (Options.Jobs is not null && Options.Jobs.Any())
        {
            foreach (var jobOptions in Options.Jobs)
            {
                var task = ScheduleAsync(jobOptions, cancellationToken);
                RunningTasks.Add(task);
            }
        }
    }

    public abstract Task ScheduleAsync(
        JobOptions options, CancellationToken cancellationToken = default);


    private static async Task AwaitStartAsync(
        JobOptions options, CancellationToken cancellationToken = default)
    {
        if (options.StartingAt is not null && options.StartingAt > DateTimeOffset.Now)
        {
            await Task.Delay(options.StartingAt.Value - DateTimeOffset.Now, cancellationToken)
                    .ConfigureAwait(false);
        }
    }

    protected async Task<(ClocJobContext, PeriodicTimer)> CreateContextAndTimerAsync(
        JobOptions options,
        CancellationToken cancellationToken = default)
    {
        var awaitStartTask = AwaitStartAsync(options, cancellationToken);

        var pollInterval = GetPollInterval(options);
        var context = GetContext(options);

        await awaitStartTask.ConfigureAwait(false);

        DateTimeOffset creationDate = (DateTimeOffset)DateTime.Today;

        if (options.DayAt.HasValue)
        {
            var curr = DateTime.Today;
            while (curr.Day != options.DayAt.Value)
            {
                curr = curr.AddDays(1);
            }
            creationDate = (DateTimeOffset)curr;
        }

        if (options.TimeAt.HasValue)
        {
            creationDate = creationDate.AddTicks(options.TimeAt.Value.Ticks);
        }

        if (creationDate > DateTimeOffset.Now)
        {
            await Task.Delay(creationDate - DateTimeOffset.Now, cancellationToken);
        }

        var timer = new PeriodicTimer(pollInterval);
        Timers.Add(timer);

        return (context, timer);
    }

    private static TimeSpan GetPollInterval(JobOptions options)
    {
        return options.Interval switch
        {
            Interval.Microsecond => TimeSpan.FromMicroseconds(options.IntervalNumber),
            Interval.Millisecond => TimeSpan.FromMilliseconds(options.IntervalNumber),
            Interval.Second => TimeSpan.FromSeconds(options.IntervalNumber),
            Interval.Minute => TimeSpan.FromMinutes(options.IntervalNumber),
            Interval.Hour => TimeSpan.FromHours(options.IntervalNumber),
            Interval.Day => TimeSpan.FromDays(options.IntervalNumber),
            Interval.Month => TimeSpan.FromDays(1),
            _ => TimeSpan.FromSeconds(1)
        };
    }

    private static ClocJobContext GetContext(JobOptions options)
    {
        ClocJobContext context = null;
        if (options.Context is not null)
        {
            try
            {
                context = new ClocJobContext(options.Context);
            }
            catch (Exception)
            {
                // Context can not be parsed, skip job
            }
        }

        return context;
    }

    public void Dispose()
    {
        foreach (var timer in Timers)
        {
            timer.Dispose();
        }
    }
}
