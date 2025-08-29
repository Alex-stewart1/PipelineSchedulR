using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Types;
using SchedulR.Pipeline;
using SchedulR.Scheduling.Helpers;
using SchedulR.Scheduling.Interfaces;

namespace SchedulR.Scheduling;

internal class ScheduledExecutable(string executableId, Type executableType, IServiceScopeFactory serviceScopeFactory) : IScheduleInterval, IScheduleExecutableConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly Type _executableType = executableType;
    private readonly string _executableId = executableId;
    private long _tickInterval;
    private DateTimeOffset _nextExecutionTime = DateTimeOffset.MaxValue;
    private bool _runOnStart = false;
    private bool _preventExecutionOverlap = false;
    public string ExecutableId => _executableId;
    public bool ShouldPreventExecutionOverlap => _preventExecutionOverlap;

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
    public void InitializeNextExecutionTime(DateTimeOffset now)
    {
        if (_runOnStart)
        {
            _nextExecutionTime = now.PreciseUpToSecond();
        }
        else
        {
            ExecutedAt(now);
        }
    }
    public IScheduleExecutableConfiguration EveryMinutes(long minutes)
    {
        _tickInterval = TickIntervalHelper.MinutesToTicks(minutes);
        return this;
    }

    public IScheduleExecutableConfiguration RunOnStart()
    {
        _runOnStart = true;
        return this;
    }

    public IScheduleExecutableConfiguration PreventExecutionOverlap()
    {
        _preventExecutionOverlap = true;
        return this;
    }
}
