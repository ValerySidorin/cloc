using Cloc;

namespace Sample.AspNetCore.Jobs
{
    public class SampleLongRunningJob : ClocJob
    {
        private readonly ILogger<SampleLongRunningJob> _logger;

        public override string Id => "LongRunning";

        public SampleLongRunningJob(ILogger<SampleLongRunningJob> logger)
        {
            _logger = logger;
        }

        public override async Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sample long running job started at: {DateTimeOffset.Now}");
            await Task.Delay(5000);
            _logger.LogInformation($"Sample long running job finished at: {DateTimeOffset.Now}");
        }
    }
}
