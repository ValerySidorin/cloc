namespace Cloc.Hosting;

public class AddClocBackgroundServiceOptions
{
    public bool WithSingletonJobs { get; set; } = true;

    public bool WithScopedJobs { get; set; } = false;

    public string ConfigSectionName { get; set; } = ClocOptions.DefaultSectionName;
}
