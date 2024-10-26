using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Registration;
using SchedulR.Scheduling;
using SchedulR.Tests.Stubs.Pipeline;

namespace SchedulR.Tests.IntegrationTests.Scheduling;

public class SchedulerIntegrationTests
{
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldExecuteDueJobs()
    {
        // Arrange
        var executableStub = new ExecutableStub1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableStub1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        // Assert
        executableStub.ExecutionTimes.Count.Should().Be(1);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldNotExecuteNotDueJobs()
    {
        // Arrange
        var executableStub = new ExecutableStub1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableStub1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddSeconds(30), CancellationToken.None);

        // Assert
        executableStub.ExecutionTimes.Count.Should().Be(0);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldExecuteDueJobsMultipleTimes()
    {
        // Arrange
        var executableStub = new ExecutableStub1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableStub1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        await scheduler.RunJobsDueAtAsync(now.AddMinutes(2), CancellationToken.None);

        // Assert
        executableStub.ExecutionTimes.Count.Should().Be(2);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerStarted_ShouldExecuteJobsDueOnStart()
    {
        // Arrange
        var executableStub = new ExecutableStub1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableStub1>()
                    .EveryMinutes(1)
                    .RunOnStart();
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        scheduler.StartAt(now);

        // Act
        await scheduler.RunJobsDueAtAsync(now, CancellationToken.None);

        // Assert
        executableStub.ExecutionTimes.Count.Should().Be(1);
    }
    [Fact]
    public async Task RunJobsDueAtAsync_WhenSchedulerNotStarted_ShouldNotExecuteJobs()
    {
        // Arrange
        var executableStub = new ExecutableStub1();
        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>();
            })
            .BuildServiceProvider()
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableStub1>()
                    .EveryMinutes(1);
            });

        var now = DateTimeOffset.UtcNow;

        var scheduler = serviceProvider.GetRequiredService<Scheduler>();

        // Act
        await scheduler.RunJobsDueAtAsync(now.AddMinutes(1), CancellationToken.None);

        // Assert
        executableStub.ExecutionTimes.Count.Should().Be(0);
    }
}
