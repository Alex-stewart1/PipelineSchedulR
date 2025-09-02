using Microsoft.Extensions.DependencyInjection;
using PipelineSchedulR.Common.Types;
using PipelineSchedulR.Pipeline;
using PipelineSchedulR.Scheduling.Helpers;
using PipelineSchedulR.Scheduling.Interfaces;

namespace PipelineSchedulR.Scheduling;

internal class ScheduledExecutable(string executableId, Type executableType, IServiceScopeFactory serviceScopeFactory) : IScheduleInterval, IScheduleExecutableConfiguration
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly Type _executableType = executableType;
    private readonly string _executableId = executableId;
    private long _tickInterval;
    private long  _jitterTicks = 0;
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
        // If RunOnStart is true, jitter is applied to the second execution
        _nextExecutionTime = now.AddSeconds(TickIntervalHelper.TicksToSeconds(_tickInterval + _jitterTicks)).PreciseUpToSecond();
        
        _jitterTicks = 0;
    }
    public void InitializeFirstExecutionTime(DateTimeOffset now)
    {
        if (_runOnStart)
        {
            _nextExecutionTime = now.PreciseUpToSecond();
        }
        else
        {
            _nextExecutionTime = now
                .AddSeconds(TickIntervalHelper.TicksToSeconds(_tickInterval + _jitterTicks))
                .PreciseUpToSecond();

            _jitterTicks = 0;
        }
    }
    public IScheduleExecutableConfiguration EveryMinutes(long minutes)
    {
        _tickInterval = TickIntervalHelper.MinutesToTicks(minutes);
        return this;
    }

    public IScheduleExecutableConfiguration EveryHours(long hours)
    {
        _tickInterval = TickIntervalHelper.HoursToTicks(hours);
        return this;
    }

    public IScheduleExecutableConfiguration EveryDays(long days)
    {
        _tickInterval = TickIntervalHelper.DaysToTicks(days);
        return this;
    }

    public IScheduleExecutableConfiguration WithJitter(TimeSpan jitter)
    {
        _jitterTicks = TickIntervalHelper.TimeSpanToTicks(jitter);
        return this;
    }

    public IScheduleExecutableConfiguration RunOnStartIf(Func<bool> predicate)
    {
        _runOnStart = predicate();
        return this;
    }

    public IScheduleExecutableConfiguration PreventExecutionOverlap()
    {
        _preventExecutionOverlap = true;
        return this;
    }
}
