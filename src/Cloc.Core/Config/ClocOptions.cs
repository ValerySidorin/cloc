namespace Cloc;

public class ClocOptions
{
    public const string DefaultSectionName = "Cloc";

    public IEnumerable<JobOptions> Jobs { get; set; }
}
