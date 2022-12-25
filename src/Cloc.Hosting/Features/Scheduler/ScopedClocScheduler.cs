using Microsoft.Extensions.DependencyInjection;

namespace Cloc.Hosting;

internal sealed class ScopedClocScheduler : ClocSchedulerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ScopedClocScheduler(
        ClocOptions options,
        IServiceProvider serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    private static ScopedClocJob GetScopedJob(IServiceScope scope, JobOptions options)
    {
        return scope.ServiceProvider.GetServices<ScopedClocJob>()
            .FirstOrDefault(j => j.Id == options.Id);
    }


    public override async Task ScheduleAsync(
        JobOptions options, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedJob = GetScopedJob(scope, options);

        if (scopedJob is null)
        {
            return;
        }

        (var context, var timer) = await CreateContextAndTimerAsync(options, cancellationToken)
            .ConfigureAwait(false);

        await scopedJob.ExecuteAsync(context, cancellationToken)
            .ConfigureAwait(false);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false) &&
            !cancellationToken.IsCancellationRequested)
        {
            if (options.DayAt.HasValue && options.DayAt.Value != DateTime.Now.Day)
            {
                return;
            }

            using var iterationScope = _serviceProvider.CreateScope();
            var job = GetScopedJob(iterationScope, options);
            if (job is null)
            {
                return;
            }

            await job.ExecuteAsync(context, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
