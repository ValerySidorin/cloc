namespace Cloc.Hosting;

public abstract class ScopedClocJob : IClocJob
{
    public abstract string Id { get; }

    public abstract Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default);
}
