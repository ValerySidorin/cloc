using Microsoft.Extensions.DependencyInjection;

namespace Cloc.Hosting;

internal class ScopedClocJobExecutor : ClocJobExecutorBase
{
    private readonly IServiceProvider _serviceProvider;

    public ScopedClocJobExecutor(
        IServiceProvider serviceProvider,
        ClocJobOptions options)
        : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var job = scope.ServiceProvider.GetServices<ScopedClocJob>()
            .FirstOrDefault(j => j.Id == Options.Id);

        if (job is null)
        {
            throw new ClocJobExecutorException(
                "Executor could not find registered job with Id: {0}", Options.Id);
        }

        await job.ExecuteAsync(Context, cancellationToken);
    }
}
