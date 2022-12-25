namespace Cloc;

public class ClocJobOptions
{
    public string Id { get; set; }

    public DateTimeOffset? StartingAt { get; set; }

    public Interval Interval { get; set; } = Interval.Second;

    public int IntervalNumber { get; set; } = 1;

    public TimeOnly? TimeAt { get; set; }

    public int? DayAt { get; set; }

    public string Context { get; set; }
}
