namespace SchedulR.Scheduling.Interfaces;

public interface IScheduleExecutableConfiguration
{
    /// <summary>
    /// Schedule the executable to run when the scheduler is started rather than waiting for its first scheduled time.
    /// </summary>
    /// <returns></returns>
    IScheduleExecutableConfiguration RunOnStart();
    /// <summary>
    /// Prevent the executable from running if it's previous exectution is still running.
    /// </summary>
    /// <returns></returns>
    IScheduleExecutableConfiguration PreventExecutionOverlap();
}
