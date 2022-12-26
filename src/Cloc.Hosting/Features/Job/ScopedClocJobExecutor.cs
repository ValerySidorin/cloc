using Cloc.Core.Exceptions;
using Cloc.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Cloc.Hosting;

internal class ScopedClocJobExecutor : IClocJobExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceCollection _services;

    public ScopedClocJobExecutor(
        IServiceProvider serviceProvider, 
        IServiceCollection services)
    {
        _serviceProvider = serviceProvider;
        _services = services;
    }

    public async Task ExecuteAsync(
        ClocJobOptions options, 
        CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var job = scope.ServiceProvider.GetServices<ScopedClocJob>()
            .FirstOrDefault(j => j.Id == options.Id);
        if (job is null)
        {
            throw new ClocJobInstanceMissingException(options.Id);
        }
        var context = options.ParseContext();
        await job.ExecuteAsync(context, cancellationToken);
    }

    public void AddJob<TClocJob>(TClocJob job)
        where TClocJob : class, IClocJob
    {
        throw new NotImplementedException();
    }
}
