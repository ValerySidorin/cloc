namespace Cloc;

internal static class ClocJobOptionsValidator
{
    public static void Validate(ClocJobOptions options)
    {
        if (options.Id is null)
        {
            throw new ClocValidationException(
                $"Job must have a defined Id property.");
        }
        if (options.IntervalNumber < 1)
        {
            throw new ClocValidationException(
                $"Interval number can not be less then 1. Cloc job Id: {options.Id}");
        }
        if (options.TimeAt is not null && options.Interval < Interval.Day)
        {
            throw new ClocValidationException(
                $"TimeAt property can not be defined with Interval: {options.Interval}. Cloc job Id: {options.Id}.");
        }
        if (options.DayOfWeekAt is not null && options.Interval < Interval.Week)
        {
            throw new ClocValidationException(
                $"DayOfWeekAt property can not be defined with Interval: {options.Interval}. Cloc job Id: {options.Id}.");
        }
        if (options.DayAt is not null && options.Interval < Interval.Month)
        {
            throw new ClocValidationException(
                $"DayAt property can not be defined with Interval: {options.Interval}. Cloc job ID: {options.Id}");
        }
    }
}
