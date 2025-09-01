using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using PipelineSchedulR.Scheduling.Helpers;

namespace PipelineSchedulR.Scheduling;

internal class SchedulerHost(
    IHostApplicationLifetime appLifetime,
    Scheduler scheduler,
    TimeProvider? timeProvider = null) : IHostedService, IDisposable
{
    private readonly IHostApplicationLifetime _appLifetime = appLifetime;
    private readonly Scheduler _scheduler = scheduler;
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    private Timer _timer = null!;
    private CancellationTokenSource _cancellationTokenSource = null!;
    private bool _shouldRun = true;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        _cancellationTokenSource = new CancellationTokenSource();

        _appLifetime.ApplicationStarted.Register(ApplicationStarted);
        _scheduler.StartRequested += SchedulerStartRequested;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _shouldRun = false;
        _cancellationTokenSource?.Cancel();
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
    private void ApplicationStarted()
    {
        if (_scheduler.AutoStart)
        {
            _scheduler.StartAt(_timeProvider.GetUtcNow());
        }
    }
    private void SchedulerStartRequested(object? sender, EventArgs e)
    {
        _timer = new Timer(RunSchedulerTick, state: null, dueTime: TimeSpan.Zero, TimeSpan.FromSeconds(TickIntervalHelper.SecondsPerTick));

        _scheduler.StartRequested -= SchedulerStartRequested;
    }

    private async void RunSchedulerTick(object? state)
    {
        try
        {
            if (_shouldRun)
            {
                await _scheduler.RunJobsDueAtAsync(_timeProvider.GetUtcNow(), _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
        } // Ignore
        catch (Exception ex)
        {
            Debugger.Break();
        }
        
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _cancellationTokenSource?.Dispose();
        _scheduler.StartRequested -= SchedulerStartRequested;
    }
}
