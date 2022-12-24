namespace Cloc;

public abstract class ClocJob : IClocJob
{
    public abstract string Id { get; }

    public abstract Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default);
}
