using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Types;
using SchedulR.Pipeline;
using SchedulR.Scheduling.Helpers;
using SchedulR.Scheduling.Interfaces;

namespace SchedulR.Scheduling;

internal class ScheduledExecutable(Type executableType, IServiceScopeFactory serviceScopeFactory) : IScheduleInterval, IScheduleExecutableConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly Type _executableType = executableType;
    private readonly string _executableId = Guid.NewGuid().ToString();
    private long _tickInterval;
    private DateTimeOffset _nextExecutionTime = DateTimeOffset.MinValue;

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
    public string GetUniqueId()
    {
        return _executableId;
    }
    public IScheduleExecutableConfiguration EveryMinutes(long minutes)
    {
        _tickInterval = TickIntervalHelper.MinutesToTicks(minutes);
        return this;
    }
}
