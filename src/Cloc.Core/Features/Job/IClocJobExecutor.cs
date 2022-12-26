namespace Cloc;

public interface IClocJobExecutor
{
    void AddJob<TClocJob>(TClocJob job = default)
        where TClocJob : class, IClocJob;

    Task ExecuteAsync(ClocJobOptions options, CancellationToken cancellationToken = default);
}
