namespace Cloc;

public interface IClocJob
{
    string Id { get; }

    Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default);
}
