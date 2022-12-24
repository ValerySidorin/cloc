namespace Cloc;

public class ClocScheduler : ClocSchedulerBase
{
    protected readonly IEnumerable<ClocJob> Jobs;

    public ClocScheduler(
        ClocOptions options,
        IEnumerable<ClocJob> jobs) : base(options)
    {
        Jobs = jobs;
    }

    public override async Task ScheduleAsync(
        JobOptions options, CancellationToken cancellationToken = default)
    {
        var clocJob = Jobs.FirstOrDefault(j => j.Id == options.Id);
        if (clocJob is null)
        {
            return;

        }
        (var context, var timer) = await CreateContextAndTimerAsync(options, cancellationToken)
            .ConfigureAwait(false);

        await clocJob.ExecuteAsync(context, cancellationToken)
            .ConfigureAwait(false);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false) &&
            !cancellationToken.IsCancellationRequested)
        {
            if (options.DayAt.HasValue && options.DayAt.Value != DateTime.Now.Day)
            {
                return;
            }
            await clocJob.ExecuteAsync(context, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
