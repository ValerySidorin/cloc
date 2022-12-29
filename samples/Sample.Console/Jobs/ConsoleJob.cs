using Cloc;

namespace Sample.Console.Jobs
{
    public class ConsoleJob : ClocJob
    {
        public override string Id => "ConsoleJob";

        public override Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default)
        {
            System.Console.WriteLine($"Console job running at: {DateTimeOffset.Now}");
            return Task.CompletedTask;
        }
    }
}
