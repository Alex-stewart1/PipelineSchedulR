namespace PipelineSchedulR.Scheduling.Interfaces;

public interface IScheduleExecutableConfiguration
{
    /// <summary>
    /// Prevent the executable from running if it's previous execution is still running.
    /// </summary>
    /// <returns></returns>
    IScheduleExecutableConfiguration PreventExecutionOverlap();
    
    
    /// <summary>
    /// Schedule the executable to run when the scheduler is started rather than waiting for its first scheduled time.
    /// </summary>
    /// <returns></returns>
    IScheduleExecutableConfiguration RunOnStartIf(Func<bool> predicate);
    
    /// <summary>
    /// Adds a jitter to the first scheduled execution time to introduce randomness in the interval.
    /// </summary>
    /// <param name="jitter" />
    /// <returns></returns>
    IScheduleExecutableConfiguration WithJitter(TimeSpan jitter);
}
