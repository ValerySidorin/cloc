using Cloc.Core.Exceptions;
using Cloc.Core.Extensions;

namespace Cloc;

public class ClocJobExecutor : IClocJobExecutor
{
    private readonly ICollection<IClocJob> _jobs;

    public ClocJobExecutor(ICollection<IClocJob> jobs)
    {
        _jobs = jobs;
    }

    public ClocJobExecutor()
    {
        _jobs = new List<IClocJob>();
    }

    public async Task ExecuteAsync(
        ClocJobOptions options, 
        CancellationToken cancellationToken = default)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == options.Id);
        if (job is null)
        {
            throw new ClocJobInstanceMissingException(options.Id);
        }
        var context = options.ParseContext();
        await job.ExecuteAsync(context, cancellationToken);
    }

    public void AddJob<TClocJob>(TClocJob job = default)
        where TClocJob : class, IClocJob
    {
        if (job != null)
        {
            _jobs.Add(job);
        }
    }
}
