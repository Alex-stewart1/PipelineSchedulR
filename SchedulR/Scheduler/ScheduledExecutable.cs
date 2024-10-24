using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Types;
using SchedulR.Pipeline;
using SchedulR.Scheduler.Helpers;
using SchedulR.Scheduler.Interfaces;

namespace SchedulR.Scheduler;

internal class ScheduledExecutable : IScheduleInterval, IScheduleExecutableConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Type _executableType;
    private readonly string _executableId;
    private long _tickInterval;
    private DateTimeOffset _nextExecutionTime = DateTimeOffset.MinValue;
    public ScheduledExecutable(Type executableType, IServiceScopeFactory serviceScopeFactory)
    {
        _executableType = executableType;
        _serviceScopeFactory = serviceScopeFactory;
        _executableId = Guid.NewGuid().ToString();
    }
    public Task<Result> ExecuteAsync(CancellationToken token)
    {
        using var asyncScope = _serviceScopeFactory.CreateAsyncScope();

        return PipelineExecutor.ExecuteAsync(_executableType, asyncScope.ServiceProvider, token);
    }
    public bool IsDue(DateTimeOffset now)
    {
        return now.PreciseUpToSecond() >= _nextExecutionTime;
    }
    public void ExecutedAt(DateTimeOffset now)
    {
        _nextExecutionTime = now.AddSeconds(TickIntervalHelper.TicksToSeconds(_tickInterval)).PreciseUpToSecond();
    }

    public IScheduleExecutableConfiguration EveryMinutes(long minutes)
    {
        _tickInterval = TickIntervalHelper.MinutesToTicks(minutes);
        return this;
    }
}
