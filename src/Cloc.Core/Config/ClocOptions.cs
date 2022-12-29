namespace Cloc;

public class ClocOptions
{
    public const string DefaultSectionName = "Cloc";

    public bool ExitOnJobFailed { get; set; } = false;

    public IEnumerable<ClocJobOptions> Jobs { get; set; }
}
