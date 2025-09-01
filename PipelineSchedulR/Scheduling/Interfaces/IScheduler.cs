using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Scheduling.Interfaces;

public interface IScheduler
{
    IScheduleInterval Schedule<TExecutable>() where TExecutable : IExecutable;
}
