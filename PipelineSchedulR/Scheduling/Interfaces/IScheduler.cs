using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Scheduling.Interfaces;

public interface IScheduler
{
    IScheduleInterval Schedule<TExecutable>(string? jobId = null) where TExecutable : IExecutable;
}
