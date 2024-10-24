namespace SchedulR.Scheduler.Interfaces;

public interface IScheduleInterval
{
    /// <summary>
    /// Schedule the executable to run every N minutes.
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    IScheduleExecutableConfiguration EveryMinutes(long minutes);
}
