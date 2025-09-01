namespace PipelineSchedulR.Scheduling.Interfaces;

public interface IScheduleInterval
{
    /// <summary>
    /// Schedule the executable to run every N minutes.
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    IScheduleStartupConfiguration EveryMinutes(long minutes);
    
    /// <summary>
    /// Schedule the executable to run every N hours.
    /// </summary>
    /// <param name="hours"></param>
    /// <returns></returns>
    IScheduleStartupConfiguration EveryHours(long hours);
   
    /// <summary>
    /// Schedule the executable to run every N days.
    /// </summary>
    /// <param name="days"></param>
    /// <returns></returns>
    IScheduleStartupConfiguration EveryDays(long days);
}
