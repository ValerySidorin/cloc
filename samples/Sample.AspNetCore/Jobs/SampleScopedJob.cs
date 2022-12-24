using Cloc;
using Cloc.Hosting;

namespace Sample.AspNetCore.Jobs
{
    public class SampleScopedJob : ScopedClocJob
    {
        private readonly ILogger<SampleScopedJob> _logger;

        public SampleScopedJob(ILogger<SampleScopedJob> logger)
        {
            _logger = logger;
        }

        public override string Id => "ScopedJob";

        public override Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Running sample scoped job at time: {DateTimeOffset.Now}");
            return Task.CompletedTask;
        }
    }
}
