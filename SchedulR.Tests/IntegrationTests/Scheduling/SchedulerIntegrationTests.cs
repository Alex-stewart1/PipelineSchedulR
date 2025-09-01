using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Registration;
using SchedulR.Scheduling;
using SchedulR.Tests.Mocks.Executable;

namespace SchedulR.Tests.IntegrationTests.Scheduling;

public class SchedulerIntegrationTests
{
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldExecuteDueJobs()
    {
        // Arrange
        var executableMock = new ExecutableMock1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(1);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldNotExecuteNotDueJobs()
    {
        // Arrange
        var executableMock = new ExecutableMock1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddSeconds(30), CancellationToken.None);

        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(0);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldExecuteDueJobsMultipleTimes()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        await scheduler.RunJobsDueAtAsync(now.AddMinutes(2), CancellationToken.None);

        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(2);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldExecuteJobsDueOnStart()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1)
                    .RunOnStart();
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now, CancellationToken.None);

        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(1);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerNotStarted_ShouldNotExecuteJobs()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(0);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_JobRunningShouldPreventOverlap_ShouldNotExecuteJob()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        try
        {
            var longRunningExecutable = new LongRunningExecutableMock(cancellationTokenSource.Token);
            var serviceProvider = new ServiceCollection()
                .AddScoped(provider => longRunningExecutable)
                .AddSchedulR((pipelineBuilder, _) =>
                {
                    pipelineBuilder
                        .Executable<LongRunningExecutableMock>();
                })
                .BuildServiceProvider()
                .UseSchedulR(scheduler =>
                {
                    scheduler
                        .Schedule<LongRunningExecutableMock>()
                        .EveryMinutes(1)
                        .RunOnStart()
                        .PreventExecutionOverlap();
                });

            var now = DateTimeOffset.UtcNow;

            var scheduler = serviceProvider.GetRequiredService<Scheduler>();

            scheduler.StartAt(now);

            // Act
            _ = scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

            _ = scheduler.RunJobsDueAtAsync(now.AddMinutes(2), CancellationToken.None);

            _ = scheduler.RunJobsDueAtAsync(now.AddMinutes(3), CancellationToken.None);

            // Assert
            longRunningExecutable.ExecutionCount.Should().Be(1);

        }
        finally
        {
            cancellationTokenSource.Cancel();
        }
    }
    [Fact]
    public async Task RunJobsDueAtAsync_JobRunningDoesNotHavePreventOverlap_ShouldExecuteJobMultipleTimes()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        try
        {
            var longRunningExecutable = new LongRunningExecutableMock(cancellationTokenSource.Token);
            var serviceProvider = new ServiceCollection()
                .AddScoped(provider => longRunningExecutable)
                .AddSchedulR((pipelineBuilder, _) =>
                {
                    pipelineBuilder
                        .Executable<LongRunningExecutableMock>();
                })
                .BuildServiceProvider()
                .UseSchedulR(scheduler =>
                {
                    scheduler
                        .Schedule<LongRunningExecutableMock>()
                        .EveryMinutes(1)
                        .RunOnStart();
                });

            var now = DateTimeOffset.UtcNow;

            var scheduler = serviceProvider.GetRequiredService<Scheduler>();

            scheduler.StartAt(now);

            // Act
            _ = scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

            _ = scheduler.RunJobsDueAtAsync(now.AddMinutes(2), CancellationToken.None);

            _ = scheduler.RunJobsDueAtAsync(now.AddMinutes(3), CancellationToken.None);

            // Assert
            longRunningExecutable.ExecutionCount.Should().Be(3);

        }
        finally
        {
            cancellationTokenSource.Cancel();
        }
    }
    [Fact]
    public async Task RunJobsDueAtAsync_CancellationTokenCancelled_ShouldCancelExecutionAndSetCancellationWasRequestedToTrue()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var executableMock = new ExecutableMock1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        cancellationTokenSource.Cancel();

        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), cancellationTokenSource.Token);

        // Assert
        executableMock.CancellationWasRequested.Should().BeTrue();
    }
    [Fact]
    public async Task RunJobsDueAtAsync_RunAfterDelay_ShouldNotExecuteJobBeforeSpecifiedDelay()
    {
        // Arrange
        var executableMock = new ExecutableMock1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1)
                    .RunAfterDelay(TimeSpan.FromMinutes(1));
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act & Assert
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        executableMock.ExecutionTimes.Count.Should().Be(0);
        
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(2), CancellationToken.None);
        
        executableMock.ExecutionTimes.Count.Should().Be(1);
    }
    
}
