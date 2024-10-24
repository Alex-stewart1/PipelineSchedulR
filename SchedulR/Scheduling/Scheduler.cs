using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchedulR.Interfaces;
using SchedulR.Scheduling.Interfaces;
using System.Collections.Concurrent;

namespace SchedulR.Scheduling;

public class Scheduler(IServiceScopeFactory serviceScopeFactory, ILogger<Scheduler> logger) : IScheduler
{
    private readonly ConcurrentDictionary<string, ScheduledExecutable> _scheduledJobs = [];
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<Scheduler> _logger = logger;

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
            if (_logger.IsEnabled(LogLevel.Trace))
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
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Executing {Executable}.", executable.ToString());
            }

            executable.ExecutedAt(now);

            await executable.ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the scheduled executable.");
        }
    }
    public IScheduleInterval Schedule<TExecutable>() where TExecutable : IExecutable
    {
        var executable = new ScheduledExecutable(typeof(TExecutable), _serviceScopeFactory);
        _scheduledJobs.TryAdd(executable.GetUniqueId(), executable);
        return executable;
    }

}
