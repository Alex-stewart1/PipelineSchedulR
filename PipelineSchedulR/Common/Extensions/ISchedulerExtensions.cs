using PipelineSchedulR.Scheduling.Interfaces;

namespace PipelineSchedulR.Common.Extensions;

public static class ISchedulerExtensions
{
    public static IScheduleExecutableConfiguration RunOnStart(this IScheduleExecutableConfiguration scheduleExecutableConfiguration)
    {
        return scheduleExecutableConfiguration.RunOnStartIf(() => true);
    }
}