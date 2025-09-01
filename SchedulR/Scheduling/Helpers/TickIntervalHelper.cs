namespace SchedulR.Scheduling.Helpers;

internal static class TickIntervalHelper
{
    internal const long MaxMinutesToTicks = 1537228672809129301; // long.MaxValue / 6
    internal const long MaxHoursToTicks = 25620477880152155; // long.MaxValue / 360
    internal const long MaxDaysToTicks = 1067519911673006; // long.MaxValue / 1440
    internal const long MaxTicksToSeconds = 922337203685477580; // long.MaxValue / 10
    internal const short SecondsPerTick = 10;
    /// <summary>
    /// Converts ticks to seconds. I.e. assuming 10 seconds per tick, 4 (ticks) would be converted to 40 (seconds).
    /// </summary>
    /// <param name="ticks"></param>
    /// <returns>
    /// Number of seconds represented by the ticks.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
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
    /// <exception cref="ArgumentOutOfRangeException"/>
    internal static long MinutesToTicks(long minutes)
    {
        if (minutes > MaxMinutesToTicks)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "The minutes is too large to be converted to ticks.");
        }

        return minutes * 6;
    }
    
    /// <summary>
    /// Converts hours to ticks. I.e. assuming 10 seconds per tick, 4 (hours) would be converted to 1440 (ticks).
    /// </summary>
    /// <param name="hours"></param>
    /// <returns>
    /// Number of ticks represented by the minutes.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    internal static long HoursToTicks(long hours)
    {
        if (hours > MaxHoursToTicks)
        {
            throw new ArgumentOutOfRangeException(nameof(hours), "The hours is too large to be converted to ticks.");
        }
        
        return hours * 360;
    }
    
    /// <summary>
    /// Converts days to ticks. I.e. assuming 10 seconds per tick, 1 (day) would be converted to 8640 (ticks).
    /// </summary>
    /// <param name="days"></param>
    /// <returns>
    /// Number of ticks represented by the minutes.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    internal static long DaysToTicks(long days)
    {
        if (days > MaxDaysToTicks)
        {
            throw new ArgumentOutOfRangeException(nameof(days), "The days is too large to be converted to ticks.");
        }
        
        return days * 1440;
    }
    
    /// <summary>
    /// Converts a TimeSpan to ticks
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    internal static long TimeSpanToTicks(TimeSpan timeSpan)
    {
        var seconds = timeSpan.TotalSeconds;
        
        return (long)seconds / SecondsPerTick;
        
    }
}
