using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Scheduling.Helpers;

public static class ScheduledExecutableHelper
{
    /// <summary>
    /// Returns a unique Id for a given executable type
    /// </summary>
    /// <typeparam name="TExecutable"></typeparam>
    /// <returns></returns>
    public static string GetExecutableId<TExecutable>() 
        where TExecutable : IExecutable 
            => typeof(TExecutable).FullName!;
}