using Cloc;
using Sample.Console.Jobs;

var cts = new CancellationTokenSource();

using var scheduler = new ClocScheduler();
var executor = new ClocJobExecutor(new ConsoleJob(), options =>
{
    options.Interval = Interval.Second;
    options.IntervalNumber = 1;
});

scheduler.Enlist(executor, true, cts.Token);

await Task.Delay(TimeSpan.FromSeconds(10));

scheduler.Stop("ConsoleJob");

await Task.Delay(TimeSpan.FromSeconds(10));
