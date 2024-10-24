namespace SchedulR.Scheduler.Helpers;

internal static class TickIntervalHelper
{
    private const long MaxMinutesToTicks = 1537228672809129301; // long.MaxValue / 6
    private const long MaxTicksToSeconds = 922337203685477580; // long.MaxValue / 10
    private const short SecondsPerTick = 10;
    /// <summary>
    /// Converts ticks to seconds. I.e. assuming 10 seconds per tick, 4 (ticks) would be converted to 40 (seconds).
    /// </summary>
    /// <param name="ticks"></param>
    /// <returns>
    /// Number of seconds represented by the ticks.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static long TicksToSeconds(long ticks)
    {
        if (ticks > MaxTicksToSeconds)
        {
            throw new ArgumentOutOfRangeException(nameof(ticks), "The ticks is too large to be converted to seconds.");
        }

        return ticks * SecondsPerTick;
    }

    /// <summary>
    /// Converts minutes to ticks. I.e. assuming 10 seconds per tick, 4 (minutes) would be converted to 24 (ticks).
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns>
    /// Number of ticks represented by the minutes.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static long MinutesToTicks(long minutes)
    {
        if (minutes > MaxMinutesToTicks)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "The minutes is too large to be converted to ticks.");
        }

        return minutes * 6;
    }
}
