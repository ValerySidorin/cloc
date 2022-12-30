# Cloc
## Overview
Yet another tiny, configurable, Task-based, thread-safe scheduling library.
## Installation
Core cloc:
```sh
PM> Install-Package Cloc.Core
```
Hosting extensions:
```sh
PM> Install-Package Cloc.Hosting
```
## Usage
**Please note, Cloc is in active development, this is not a final list of features.**
First of all, you have to define your job by implementing cloc job abstract class:
```csharp
public class ConsoleJob : ClocJob
    {
        public override string Id => "ConsoleJob";

        public override Task ExecuteAsync(ClocJobContext context, CancellationToken cancellationToken = default)
        {
            System.Console.WriteLine($"Console job running at: {DateTimeOffset.Now}");
            return Task.CompletedTask;
        }
    }
```
### Console
Just instantiate scheduler, enlist your job with options, and start:
```csharp
var cts = new CancellationTokenSource();
using var scheduler = new ClocScheduler();
var executor = new ClocJobExecutor(new ConsoleJob(), options =>
{
    options.Interval = Interval.Second;
    options.IntervalNumber = 1;
});

scheduler.Enlist(executor);
scheduler.Start(cts.Token);
await Task.Delay(TimeSpan.FromSeconds(10));

// Don't forget to stop or dispose scheduler to release all its internal timers
scheduler.Stop();
// scheduler.Dispose();
await Task.Delay(TimeSpan.FromSeconds(10));
```
### AspNetCore
Cloc works great with .Net dependency injection toolkit. It can execute both singleton and scoped jobs. Scoped job example below:
```csharp
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
```
Note, that you need to define cloc options in your appsettings.json config file:
```json
{
  "Cloc": {
    "ExitOnJobFailed": false,
    "Jobs": [
      {
        "Id": "ScopedJob",
        "Interval": "Second",
        "IntervalNumber": 1
      },
      {
        "Id": "SampleJob",
        "Interval": "Day",
        "IntervalNumber": 1,
        "TimeAt": "12:00"
      }
    ]
  }
}
```
Then just register cloc:
```csharp
builder.Services.AddCloc(builder.Configuration);
```
You don't need to register all jobs, they are being added dynamically from your calling assembly and it referenced ones.