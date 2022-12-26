using Cloc.Core.Exceptions;

namespace Cloc.Core.Extensions
{
    public static class ClocJobOptionsExtensions
    {
        public static ClocJobContext ParseContext(this ClocJobOptions options)
        {
            if (options.Context is not null)
            {
                try
                {
                    return new ClocJobContext(options.Context);
                }
                catch (Exception ex)
                {
                    throw new ClocJobContextParsingException(ex);
                }
            }
            return null;
        }

        public static TimeSpan GetPollInterval(this ClocJobOptions options)
        {
            return options.Interval switch
            {
                Interval.Microsecond => TimeSpan.FromMicroseconds(options.IntervalNumber),
                Interval.Millisecond => TimeSpan.FromMilliseconds(options.IntervalNumber),
                Interval.Second => TimeSpan.FromSeconds(options.IntervalNumber),
                Interval.Minute => TimeSpan.FromMinutes(options.IntervalNumber),
                Interval.Hour => TimeSpan.FromHours(options.IntervalNumber),
                Interval.Day => TimeSpan.FromDays(options.IntervalNumber),
                Interval.Week => TimeSpan.FromDays(Constants.DaysInWeek * options.IntervalNumber),
                Interval.Month => TimeSpan.FromDays(1),
                _ => TimeSpan.FromSeconds(1)
            };
        }

        public static DateTimeOffset GetStartingPointOfTime(this ClocJobOptions options)
        {
            DateTimeOffset startAt = options.StartingAt ?? (DateTimeOffset)DateTime.Today;

            if (options.DayAt.HasValue)
            {
                var curr = startAt;
                while (curr.Day != options.DayAt.Value)
                {
                    curr = curr.AddDays(1);
                }
                startAt = curr;
            }

            if (options.DayOfWeekAt.HasValue)
            {
                var curr = startAt;
                while (curr.DayOfWeek != options.DayOfWeekAt.Value)
                {
                    curr = curr.AddDays(1);
                }
                startAt = curr;
            }

            if (options.TimeAt.HasValue)
            {
                startAt = startAt.AddTicks(options.TimeAt.Value.Ticks);
            }

            return startAt;
        }
    }
}
