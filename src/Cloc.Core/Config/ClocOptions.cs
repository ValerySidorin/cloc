namespace Cloc;

public class ClocOptions
{
    public const string DefaultSectionName = "Cloc";

    public IEnumerable<ClocJobOptions> Jobs { get; set; }
}
