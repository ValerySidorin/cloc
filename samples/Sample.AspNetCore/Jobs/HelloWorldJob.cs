using Cloc;
using Cloc.Hosting;

namespace Sample.AspNetCore.Jobs
{
    public class HelloWorldJob : ScopedClocJob
    {
        private readonly ILogger<HelloWorldJob> _logger;

        public HelloWorldJob(ILogger<HelloWorldJob> logger)
        {
            _logger = logger;
        }

        public override string Id => "HelloWorldJob";

        public override Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default)
        {
            var id = context.GetInt("Id");
            _logger.LogInformation($"Id: {id}. Hello world job running at: {DateTimeOffset.Now}");
            return Task.CompletedTask;

        }
    }
}
