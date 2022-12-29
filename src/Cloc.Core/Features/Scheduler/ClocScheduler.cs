using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Cloc;

public class ClocScheduler : IDisposable
{
    public readonly ICollection<ClocJobExecutorBase> Executors;

    private readonly ICollection<NamedPeriodicTimer> _timers;

    private readonly ObservableCollection<Task> _runningTasks;

    private readonly ICollection<string> _runningJobs;

    private readonly object _lock = new object();

    public ClocScheduler(
        bool exitOnJobFailed = false, 
        ICollection<ClocJobExecutorBase> executors = null)
    {
        Executors = executors ?? new List<ClocJobExecutorBase>();
        _timers = new List<NamedPeriodicTimer>();

        _runningTasks = new ObservableCollection<Task>();
        _runningJobs = new List<string>();

        if (exitOnJobFailed)
        {
            _runningTasks.CollectionChanged += CatchInnerException;
        }
    }

    public void Enlist(ClocJobExecutorBase executor, bool startImmediately = false, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            Debug.Assert(executor is not null);
            if (!_runningJobs.Any(j => j == executor.Options.Id))
            {
                Executors.Add(executor);
                if (startImmediately)
                {
                    _runningTasks.Add(ScheduleAsync(executor, cancellationToken));
                    _runningJobs.Add(executor.Options.Id);
                }
            }
            else
            {
                throw new ClocJobExecutorException("Job with Id: {0} already enlisted.", executor.Options.Id);
            }
        }
    }

    public void Start(string jobId, CancellationToken cancellationToken = default)
    {
        lock (_lock )
        {
            var executor = Executors.FirstOrDefault(e => e.Options.Id == jobId);
            if (executor is null)
            {
                throw new ClocJobExecutorException("Executor for job with Id: {0} not found.", jobId);
            }
            if (!_runningJobs.Any(j => j == jobId))
            {
                _runningTasks.Add(ScheduleAsync(executor, cancellationToken));
                _runningJobs.Add(jobId);
            }
        }
    }

    public void Start(CancellationToken cancellationToken = default)
    {
        lock (_lock )
        {
            foreach (var executor in Executors)
            {
                if (!_runningJobs.Any(j => j == executor.Options.Id))
                {
                    _runningTasks.Add(ScheduleAsync(executor, cancellationToken));
                    _runningJobs.Add(executor.Options.Id);
                }
            }
        }
    }

    public void Stop()
    {
        Dispose();
    }

    public void Stop(string jobId)
    {
        lock (_lock)
        {
            var executor = Executors.FirstOrDefault(e => e.Options.Id == jobId);
            if (executor is null)
            {
                return;
            }
            if (!_runningJobs.Any(j => j == jobId))
            {
                return;
            }
            var timer = _timers.FirstOrDefault(t => t.Name == jobId);
            if (timer is null)
            {
                return;
            }
            timer.Dispose();
        }
    }

    private void CatchInnerException(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (Task task in e.NewItems)
            {
                if (task.IsFaulted)
                {
                    throw new ClocException(task.Exception);
                }
            }
        }
    }

    private async Task ScheduleAsync(
        ClocJobExecutorBase executor,
        CancellationToken cancellationToken = default)
    {
        var startAt = executor.Options.GetStartingPointOfTime();
        await DelayStartAsync(startAt, cancellationToken).ConfigureAwait(false);

        var interval = executor.Options.GetPollInterval();

        var timer = new NamedPeriodicTimer(interval, executor.Options.Id);
        _timers.Add(timer);

        await executor.ExecuteAsync(cancellationToken)
            .ConfigureAwait(false);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false) &&
            !cancellationToken.IsCancellationRequested)
        {
            if (executor.Options.DayAt.HasValue && executor.Options.DayAt.Value != DateTime.Now.Day)
            {
                return;
            }

            await executor.ExecuteAsync(cancellationToken)
            .ConfigureAwait(false);
        }
        _timers.Remove(timer);
        timer.Dispose();
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
        lock (_lock)
        {
            while (_timers.Count > 0)
            {
                var timer = _timers.FirstOrDefault();
                timer.Dispose();
                _timers.Remove(timer);
            }
        }
    }
}
