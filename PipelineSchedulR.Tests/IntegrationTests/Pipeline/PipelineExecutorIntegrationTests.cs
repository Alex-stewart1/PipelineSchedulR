using Microsoft.Extensions.DependencyInjection;
using PipelineSchedulR.Common.Registration;
using PipelineSchedulR.Pipeline;
using PipelineSchedulR.Tests.Mocks.Executable;
using PipelineSchedulR.Tests.Mocks.Pipeline;

namespace PipelineSchedulR.Tests.IntegrationTests.Pipeline;

public class PipelineExecutorIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCalled_ShouldReturnExecutableResult()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var pipelineMock = new PipelineMock1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddScoped(provider => pipelineMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>()
                    .WithPipeline<PipelineMock1>();
            })
            .BuildServiceProvider();

        // Act
        var result = await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock1), serviceProvider, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalled_ShouldCorrectlyActivatePipeline()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var pipelineMock = new PipelineMock1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddScoped(provider => pipelineMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>()
                    .WithPipeline<PipelineMock1>();
            })
            .BuildServiceProvider();

        // Act
        await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock1), serviceProvider, CancellationToken.None);

        // Assert
        pipelineMock.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock.AfterExecutionTime.Should().NotBeNull();
        executableMock.ExecutionTimes.Should().HaveCount(1);
        pipelineMock.BeforeExecutionTime.Should().BeBefore(executableMock.ExecutionTimes[0]);
        executableMock.ExecutionTimes[0].Should().BeBefore(pipelineMock.AfterExecutionTime!.Value);
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalled_ShouldPassExecutableTypeToPipeline()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var pipelineMock = new PipelineMock1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddScoped(provider => pipelineMock)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>()
                    .WithPipeline<PipelineMock1>();
            })
            .BuildServiceProvider();

        // Act
        await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock1), serviceProvider, CancellationToken.None);
        
        // Assert
        pipelineMock.ExecutableType.Should().Be<ExecutableMock1>();
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalledWithNoPipeline_ShouldReturnExecutableResult()
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
            .BuildServiceProvider();

        // Act
        var result = await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock1), serviceProvider, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalledWithMultiplePipelines_ShouldCorrectlyActivatePipelines()
    {
        // Arrange
        var executableMock = new ExecutableMock1();
        var pipelineMock1 = new PipelineMock1();
        var pipelineMock2 = new PipelineMock2();
        var pipelineMock3 = new PipelineMock3();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock)
            .AddScoped(provider => pipelineMock1)
            .AddScoped(provider => pipelineMock2)
            .AddScoped(provider => pipelineMock3)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>()
                    .WithPipeline<PipelineMock1>()
                    .WithPipeline<PipelineMock2>()
                    .WithPipeline<PipelineMock3>();
            })
            .BuildServiceProvider();

        // Act
        var result = await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock1), serviceProvider, CancellationToken.None);

        // Assert

        // Executable
        result.IsSuccess.Should().BeTrue();
        executableMock.ExecutionTimes.Should().HaveCount(1);

        // Pipeline 3
        pipelineMock3.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock3.AfterExecutionTime.Should().NotBeNull();
        pipelineMock2.BeforeExecutionTime.Should().BeBefore(executableMock.ExecutionTimes[0]);
        pipelineMock2.AfterExecutionTime.Should().BeAfter(executableMock.ExecutionTimes[0]);

        // Pipeline 2
        pipelineMock3.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock3.AfterExecutionTime.Should().NotBeNull();
        pipelineMock2.BeforeExecutionTime.Should().BeBefore(pipelineMock3.BeforeExecutionTime!.Value);
        pipelineMock2.AfterExecutionTime.Should().BeAfter(pipelineMock3.AfterExecutionTime!.Value);

        // Pipeline 1
        pipelineMock1.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock1.AfterExecutionTime.Should().NotBeNull();
        pipelineMock1.BeforeExecutionTime.Should().BeBefore(pipelineMock2.BeforeExecutionTime!.Value);
        pipelineMock1.AfterExecutionTime.Should().BeAfter(pipelineMock2.AfterExecutionTime!.Value);
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalledWithMultipleRegisteredExecutables_ShouldActivateTheCorrectExecutablePipeline()
    {
        // Arrange
        var executableMock1 = new ExecutableMock1();
        var pipelineMock1 = new PipelineMock1();

        var executableMock2 = new ExecutableMock2();
        var pipelineMock2 = new PipelineMock2();
        var pipelineMock3 = new PipelineMock3();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableMock1)
            .AddScoped(provider => pipelineMock1)
            .AddScoped(provider => executableMock2)
            .AddScoped(provider => pipelineMock2)
            .AddScoped(provider => pipelineMock3)
            .AddSchedulR((pipelineBuilder, _) =>
            {
                pipelineBuilder
                    .Executable<ExecutableMock1>()
                    .WithPipeline<PipelineMock1>();

                pipelineBuilder
                    .Executable<ExecutableMock2>()
                    .WithPipeline<PipelineMock2>()
                    .WithPipeline<PipelineMock3>();
            })
            .BuildServiceProvider();

        // Act & Assert
        await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock1), serviceProvider, CancellationToken.None);

        executableMock1.ExecutionTimes.Should().HaveCount(1);
        pipelineMock1.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock1.AfterExecutionTime.Should().NotBeNull();

        executableMock2.ExecutionTimes.Should().BeEmpty();
        pipelineMock2.BeforeExecutionTime.Should().BeNull();
        pipelineMock2.AfterExecutionTime.Should().BeNull();
        pipelineMock3.BeforeExecutionTime.Should().BeNull();
        pipelineMock3.AfterExecutionTime.Should().BeNull();

        await PipelineExecutor.ExecuteAsync(typeof(ExecutableMock2), serviceProvider, CancellationToken.None);

        executableMock2.ExecutionTimes.Should().HaveCount(1);
        pipelineMock2.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock2.AfterExecutionTime.Should().NotBeNull();
        pipelineMock3.BeforeExecutionTime.Should().NotBeNull();
        pipelineMock3.AfterExecutionTime.Should().NotBeNull();
    }
    
    
}
