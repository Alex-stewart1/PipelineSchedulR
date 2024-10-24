using SchedulR.Interfaces;

namespace SchedulR.Scheduling.Interfaces;

public interface IScheduler
{
    IScheduleInterval Schedule<TExecutable>() where TExecutable : IExecutable;
}
