using System.Diagnostics;

namespace Cloc;

public abstract class ClocJobExecutorBase
{
    protected Lazy<ClocJobContext> LazyContext { get; init; }

    public ClocJobOptions Options { get; init; }

    protected ClocJobContext Context => LazyContext.Value;

    public ClocJobExecutorBase(ClocJobOptions options)
    {
        Debug.Assert(options is not null);
        ClocJobOptionsValidator.Validate(options);

        LazyContext = new Lazy<ClocJobContext>(() => options.ParseContext());
        Options = options;
    }

    public ClocJobExecutorBase(Action<ClocJobOptions> configure)
    {
        var options = new ClocJobOptions();
        configure(options);
        ClocJobOptionsValidator.Validate(options);

        LazyContext = new Lazy<ClocJobContext>(() => options.ParseContext());

        Options = options;
    }

    public ClocJobExecutorBase()
    {

    }

    public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);

    
}
