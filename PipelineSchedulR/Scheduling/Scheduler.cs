using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipelineSchedulR.Interfaces;
using PipelineSchedulR.Scheduling.Configuration;
using PipelineSchedulR.Scheduling.Interfaces;
using PipelineSchedulR.Scheduling.Mutex;
using System.Collections.Concurrent;
using System.Diagnostics;
using PipelineSchedulR.Scheduling.Helpers;

namespace PipelineSchedulR.Scheduling;

public class Scheduler(IServiceScopeFactory serviceScopeFactory,
                       SchedulerOptions options,
                       ILogger<Scheduler>? logger = null) : IScheduler
{
    #region Fields
    private bool _hasStarted = false;
    private readonly Lock _startLock = new();
    private readonly Lock _runJobsLock = new();
    #endregion

    #region Properties
    public bool AutoStart { get; } = options.AutoStart;
    public bool HasStarted => _hasStarted;
    public IEnumerable<string> ScheduledJobs => _scheduledJobs.Keys;
    #endregion

    #region Dependencies
    private readonly ConcurrentDictionary<string, ScheduledExecutable> _scheduledJobs = [];
    private readonly ExecutableMutex _mutex = new();
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<Scheduler>? _logger = logger;
    #endregion

    #region Events
    internal event EventHandler? StartRequested = null;
    #endregion

    internal Task RunJobsDueAtAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        List<ScheduledExecutable>? dueExecutables = null;

        lock (_runJobsLock)
        {
            // Get all due executables
            foreach (var executable in _scheduledJobs.Values)
            {
                // Ensures any jobs added after Scheduler.Start() is called are initialized (im not a huge fan of this)
                if (!executable.Initialized)
                {
                    executable.InitializeFirstExecutionTime(now);
                }
            
                if (executable.IsDue(now))
                {
                    dueExecutables ??= [];

                    dueExecutables.Add(executable);
                
                    executable.ExecutedAt(now);
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
        }

        // Execute all due executables
        return Task.WhenAll(dueExecutables.Select(executable => RunExecutableAtAsync(executable, cancellationToken)));
    }

    private async Task RunExecutableAtAsync(ScheduledExecutable executable, CancellationToken cancellationToken)
    {
        try
        {
            if (executable.ShouldPreventExecutionOverlap)
            {
                if (_mutex.TryAcquire(executable.ExecutableId))
                {
                    try
                    {
                        if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
                        {
                            _logger.LogDebug("Executing {Executable}.", executable.ToString());
                        }

                        await executable.ExecuteAsync(cancellationToken);
                    }
                    finally
                    {
                        _mutex.Release(executable.ExecutableId);
                    }
                }
            }
            else
            {
                if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
                {
                    _logger.LogDebug("Executing {Executable}.", executable.ToString());
                }

                await executable.ExecuteAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException) { } // Ignore
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while executing the scheduled executable.");
            
            Debugger.Break();
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
                executable.InitializeFirstExecutionTime(now);
            }

            StartRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Schedules a new job of the specified executable type.
    /// </summary>
    /// <typeparam name="TExecutable">The type of executable to schedule.</typeparam>
    /// <param name="jobId">The optional job identifier. If not provided, a default Id will be used.</param>
    /// <returns>An instance of <see cref="IScheduleInterval"/> that allows configuring the scheduling interval for the job.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if an executable with the same ID has already been scheduled.
    /// </exception>
    public IScheduleInterval Schedule<TExecutable>(string? jobId = null) where TExecutable : IExecutable
    {
        var executableId = jobId ?? ScheduledExecutableHelper.GetExecutableId<TExecutable>();
        var executable = new ScheduledExecutable(executableId, typeof(TExecutable), _serviceScopeFactory);
        if (!_scheduledJobs.TryAdd(executable.ExecutableId, executable))
        {
            throw new InvalidOperationException($"An executable with the id {executableId} has already been scheduled.");       
        }
        return executable;
    }

    /// <summary>
    /// Removes a scheduled job with the specified job Id from the scheduler.
    /// </summary>
    /// <param name="jobId">The identifier of the job to be removed from the scheduler.</param>
    public void Deschedule(string jobId)
    {
        _scheduledJobs.TryRemove(jobId, out _);
    }

    /// <summary>
    ///  Removes a job based on its type
    /// </summary>
    public void Deschedule<TExecutable>() where TExecutable : IExecutable
    {
        var executableId = ScheduledExecutableHelper.GetExecutableId<TExecutable>();
        Deschedule(executableId);   
    }
}
