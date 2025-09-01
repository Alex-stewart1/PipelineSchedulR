namespace PipelineSchedulR.Scheduling.Interfaces;

public interface IScheduleStartupConfiguration
{
    /// <summary>
    /// Schedule the executable to run when the scheduler is started rather than waiting for its first scheduled time.
    /// </summary>
    /// <returns></returns>
    IScheduleExecutableConfiguration RunOnStart();
    
    
    /// <summary>
    /// Schedule the executable to run after the specified delay once the scheduler has started
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IScheduleExecutableConfiguration RunAfterDelay(TimeSpan delay);
}