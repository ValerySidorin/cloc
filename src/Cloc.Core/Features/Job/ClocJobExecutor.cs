using System.Diagnostics;

namespace Cloc;

public class ClocJobExecutor : ClocJobExecutorBase
{
    private readonly ClocJob _job;

    public ClocJobExecutor(ClocJob job, ClocJobOptions options)
        : base(options)
    {
        Debug.Assert(job is not null);

        _job = job;
        Options = options;
        Options.Id = job.Id;
        LazyContext = new Lazy<ClocJobContext>(() => options.ParseContext());
        ClocJobOptionsValidator.Validate(options);
    }

    public ClocJobExecutor(ClocJob job, Action<ClocJobOptions> configure)
    {
        _job = job;
        var options = new ClocJobOptions();
        configure(options);

        options.Id = job.Id;
        Options = options;
        LazyContext = new Lazy<ClocJobContext>(() => options.ParseContext());
        ClocJobOptionsValidator.Validate(Options);
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await _job.ExecuteAsync(Context, cancellationToken);
    }
}
