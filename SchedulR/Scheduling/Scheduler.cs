using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchedulR.Interfaces;
using SchedulR.Scheduling.Configuration;
using SchedulR.Scheduling.Interfaces;
using SchedulR.Scheduling.Mutex;
using System.Collections.Concurrent;

namespace SchedulR.Scheduling;

public class Scheduler(IServiceScopeFactory serviceScopeFactory,
                       SchedulerOptions options,
                       ILogger<Scheduler>? logger = null) : IScheduler
{
    #region Fields
    private bool _hasStarted = false;
    private readonly object _startLock = new();
    #endregion

    #region Properties
    public bool AutoStart { get; } = options.AutoStart;
    public bool HasStarted => _hasStarted;
    #endregion

    #region Dependencies
    private readonly ConcurrentDictionary<string, ScheduledExecutable> _scheduledJobs = [];
    private readonly ExecutableMutex _mutex = new();
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<Scheduler>? _logger = logger;
    #endregion

    #region Events
    public event EventHandler? StartRequested = null;
    #endregion

    public Task RunJobsDueAtAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        List<ScheduledExecutable>? dueExecutables = null;

        // Get all due executables
        foreach (var executable in _scheduledJobs.Values)
        {
            if (executable.IsDue(now))
            {
                dueExecutables ??= [];

                dueExecutables.Add(executable);
            }
        }

        // No due executables
        if (dueExecutables is null)
        {
            if (_logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                _logger.LogTrace("No jobs due.");
            }

            return Task.CompletedTask;
        }

        // Execute all due executables
        return Task.WhenAll(dueExecutables.Select(executable => RunExecutableAtAsync(now, executable, cancellationToken)));
    }

    private async Task RunExecutableAtAsync(DateTimeOffset now, ScheduledExecutable executable, CancellationToken cancellationToken)
    {
        try
        {
            if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                _logger.LogDebug("Executing {Executable}.", executable.ToString());
            }

            executable.ExecutedAt(now);

            await executable.ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while executing the scheduled executable.");
        }
    }
    /// <summary>
    /// Start the scheduler.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Start() => StartAt(DateTimeOffset.Now);

    /// <summary>
    /// Start the scheduler at the specified time.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void StartAt(DateTimeOffset now)
    {
        lock (_startLock)
        {
            if (_hasStarted)
            {
                throw new InvalidOperationException("The scheduler has already started.");
            }

            _hasStarted = true;

            foreach (var executable in _scheduledJobs.Values)
            {
                executable.InitializeNextExecutionTime(now);
            }

            StartRequested?.Invoke(this, EventArgs.Empty);
        }
    }
    public IScheduleInterval Schedule<TExecutable>() where TExecutable : IExecutable
    {
        var executable = new ScheduledExecutable(typeof(TExecutable), _serviceScopeFactory);
        _scheduledJobs.TryAdd(executable.ExecutableId, executable);
        return executable;
    }

}
