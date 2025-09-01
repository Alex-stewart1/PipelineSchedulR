namespace SchedulR.Scheduling.Interfaces;

public interface IScheduleExecutableConfiguration
{

    /// <summary>
    /// Prevent the executable from running if it's previous execution is still running.
    /// </summary>
    /// <returns></returns>
    IScheduleExecutableConfiguration PreventExecutionOverlap();
}
