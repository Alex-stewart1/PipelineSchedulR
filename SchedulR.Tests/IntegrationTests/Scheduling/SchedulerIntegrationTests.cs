using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Registration;
using SchedulR.Scheduling;
using SchedulR.Tests.Stubs.Pipeline;

namespace SchedulR.Tests.IntegrationTests.Scheduling;

public class SchedulerIntegrationTests
{
    [Fact]
    public async Task RunJobsDueAtAsync_WhenCalled_ShouldExecuteDueJobs()
    {
        // Arrange
        var executableStub = new ExecutableStub1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddSchedulR(pipelineBuilder =>
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
        await scheduler.RunJobsDueAtAsync(now, CancellationToken.None);

        now.AddMinutes(1);

        await scheduler.RunJobsDueAtAsync(now, CancellationToken.None);

        // Assert
        executableStub.ExecutionTimes.Count.Should().Be(2);



    }
}
