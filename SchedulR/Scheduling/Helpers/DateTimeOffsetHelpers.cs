namespace SchedulR.Scheduling.Helpers;

internal static class DateTimeOffsetHelpers
{
    internal static DateTimeOffset PreciseUpToSecond(this DateTimeOffset dt)
    {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Offset);
    }
}
