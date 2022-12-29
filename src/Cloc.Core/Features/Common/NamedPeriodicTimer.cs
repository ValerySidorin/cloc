namespace Cloc;

internal class NamedPeriodicTimer : IDisposable
{
    private readonly PeriodicTimer _timer;
    public string Name { get; }

    public NamedPeriodicTimer(TimeSpan timeSpan, string name)
    {
        _timer = new PeriodicTimer(timeSpan);
        Name = name;
    }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default) =>
        _timer.WaitForNextTickAsync(cancellationToken);

    public void Dispose()
    {
        _timer.Dispose();
    }
}
