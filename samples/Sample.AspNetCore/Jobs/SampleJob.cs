using Cloc;

namespace Sample.AspNetCore.Jobs
{
    public class SampleJob : ClocJob
    {
        private readonly ILogger<SampleJob> _logger;

        public SampleJob(ILogger<SampleJob> logger)
        {
            _logger = logger;
        }

        public override string Id => "SampleJob";

        public override Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Scheduler running sample job at time: {DateTimeOffset.Now}");
            return Task.CompletedTask;
        }
    }
}
