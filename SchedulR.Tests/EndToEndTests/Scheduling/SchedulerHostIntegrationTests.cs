using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchedulR.Common.Registration;
using SchedulR.Tests.Mocks.Executable;

namespace SchedulR.Tests.EndToEndTests.Scheduling;

public class SchedulerHostIntegrationTests
{
    [Fact]
    public async Task HostApplicationStarted_AutoStartIsTrue_ShouldStartScheduler()
    {
        // Arrange 
        var executableMock = new ExecutableMock1();

        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddScoped(provider => executableMock)
                    .AddSchedulR((pipelineBuilder, options) =>
                    {
                        options.AutoStart = true;
                        pipelineBuilder
                            .Executable<ExecutableMock1>();
                    });
            })
            .Build();

        host.Services
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1)
                    .RunOnStart();
            });
        
        // Act
        _ = host.RunAsync();
        
        // TODO: Properly allow for the Timer in the SchedulerHost to be mocked in some way
        // this will prevent the test from needing to wait for the timer to trigger
        await Task.Delay(TimeSpan.FromSeconds(1)); 
        
        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task HostApplicationStarted_AutoStartIsFalse_ShouldNotStartScheduler()
    {
        // Arrange 
        var executableMock = new ExecutableMock1();

        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddScoped(provider => executableMock)
                    .AddSchedulR((pipelineBuilder, options) =>
                    {
                        options.AutoStart = false;
                        pipelineBuilder
                            .Executable<ExecutableMock1>();
                    });
            })
            .Build();

        host.Services
            .UseSchedulR(scheduler =>
            {
                scheduler
                    .Schedule<ExecutableMock1>()
                    .EveryMinutes(1)
                    .RunOnStart();
            });
        
        // Act
        _ = host.StartAsync();
        
        // TODO: Properly allow for the Timer in the SchedulerHost to be mocked in some way
        // this will prevent the test from needing to wait for the timer to trigger
        await Task.Delay(TimeSpan.FromSeconds(1)); 
        
        // Assert
        executableMock.ExecutionTimes.Count.Should().Be(0);
    }
}