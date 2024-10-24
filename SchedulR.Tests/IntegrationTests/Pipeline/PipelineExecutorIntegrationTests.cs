using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Registration;
using SchedulR.Pipeline;
using SchedulR.Tests.Stubs.Pipeline;

namespace SchedulR.Tests.IntegrationTests.Pipeline;

public class PipelineExecutorIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCalled_ShouldReturnExecutableResult()
    {
        // Arrange
        var executableStub = new ExecutableStub1();
        var pipelineStub = new PipelineStub1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddScoped(provider => pipelineStub)
            .AddSchedulR(pipelineBuilder =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>()
                    .WithPipeline<PipelineStub1>();
            })
            .BuildServiceProvider();

        // Act
        var result = await PipelineExecutor.ExecuteAsync(typeof(ExecutableStub1), serviceProvider, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalled_ShouldCorrectlyActivatePipeline()
    {
        // Arrange
        var executableStub = new ExecutableStub1();
        var pipelineStub = new PipelineStub1();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddScoped(provider => pipelineStub)
            .AddSchedulR(pipelineBuilder =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>()
                    .WithPipeline<PipelineStub1>();
            })
            .BuildServiceProvider();

        // Act
        await PipelineExecutor.ExecuteAsync(typeof(ExecutableStub1), serviceProvider, CancellationToken.None);

        // Assert
        pipelineStub.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub.AfterExecutionTime.Should().NotBeNull();
        executableStub.ExecutionTime.Should().NotBeNull();
        pipelineStub.BeforeExecutionTime.Should().BeBefore(executableStub.ExecutionTime!.Value);
        executableStub.ExecutionTime.Should().BeBefore(pipelineStub.AfterExecutionTime!.Value);
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalledWithNoPipeline_ShouldReturnExecutableResult()
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
            .BuildServiceProvider();

        // Act
        var result = await PipelineExecutor.ExecuteAsync(typeof(ExecutableStub1), serviceProvider, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalledWithMultiplePipelines_ShouldCorrectlyActivatePipelines()
    {
        // Arrange
        var executableStub = new ExecutableStub1();
        var pipelineStub1 = new PipelineStub1();
        var pipelineStub2 = new PipelineStub2();
        var pipelineStub3 = new PipelineStub3();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub)
            .AddScoped(provider => pipelineStub1)
            .AddScoped(provider => pipelineStub2)
            .AddScoped(provider => pipelineStub3)
            .AddSchedulR(pipelineBuilder =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>()
                    .WithPipeline<PipelineStub1>()
                    .WithPipeline<PipelineStub2>()
                    .WithPipeline<PipelineStub3>();
            })
            .BuildServiceProvider();

        // Act
        var result = await PipelineExecutor.ExecuteAsync(typeof(ExecutableStub1), serviceProvider, CancellationToken.None);

        // Assert

        // Executable
        result.IsSuccess.Should().BeTrue();
        executableStub.ExecutionTime.Should().NotBeNull();

        // Pipeline 3
        pipelineStub3.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub3.AfterExecutionTime.Should().NotBeNull();
        pipelineStub2.BeforeExecutionTime.Should().BeBefore(executableStub.ExecutionTime!.Value);
        pipelineStub2.AfterExecutionTime.Should().BeAfter(executableStub.ExecutionTime!.Value);

        // Pipeline 2
        pipelineStub3.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub3.AfterExecutionTime.Should().NotBeNull();
        pipelineStub2.BeforeExecutionTime.Should().BeBefore(pipelineStub3.BeforeExecutionTime!.Value);
        pipelineStub2.AfterExecutionTime.Should().BeAfter(pipelineStub3.AfterExecutionTime!.Value);

        // Pipeline 1
        pipelineStub1.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub1.AfterExecutionTime.Should().NotBeNull();
        pipelineStub1.BeforeExecutionTime.Should().BeBefore(pipelineStub2.BeforeExecutionTime!.Value);
        pipelineStub1.AfterExecutionTime.Should().BeAfter(pipelineStub2.AfterExecutionTime!.Value);
    }
    [Fact]
    public async Task ExecuteAsync_WhenCalledWithMultipleRegisteredExecutables_ShouldActivateTheCorrectExecutablePipeline()
    {
        // Arrange
        var executableStub1 = new ExecutableStub1();
        var pipelineStub1 = new PipelineStub1();

        var executableStub2 = new ExecutableStub2();
        var pipelineStub2 = new PipelineStub2();
        var pipelineStub3 = new PipelineStub3();

        var serviceProvider = new ServiceCollection()
            .AddScoped(provider => executableStub1)
            .AddScoped(provider => pipelineStub1)
            .AddScoped(provider => executableStub2)
            .AddScoped(provider => pipelineStub2)
            .AddScoped(provider => pipelineStub3)
            .AddSchedulR(pipelineBuilder =>
            {
                pipelineBuilder
                    .Executable<ExecutableStub1>()
                    .WithPipeline<PipelineStub1>();

                pipelineBuilder
                    .Executable<ExecutableStub2>()
                    .WithPipeline<PipelineStub2>()
                    .WithPipeline<PipelineStub3>();
            })
            .BuildServiceProvider();

        // Act & Assert
        await PipelineExecutor.ExecuteAsync(typeof(ExecutableStub1), serviceProvider, CancellationToken.None);

        executableStub1.ExecutionTime.Should().NotBeNull();
        pipelineStub1.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub1.AfterExecutionTime.Should().NotBeNull();

        executableStub2.ExecutionTime.Should().BeNull();
        pipelineStub2.BeforeExecutionTime.Should().BeNull();
        pipelineStub2.AfterExecutionTime.Should().BeNull();
        pipelineStub3.BeforeExecutionTime.Should().BeNull();
        pipelineStub3.AfterExecutionTime.Should().BeNull();

        await PipelineExecutor.ExecuteAsync(typeof(ExecutableStub2), serviceProvider, CancellationToken.None);

        executableStub2.ExecutionTime.Should().NotBeNull();
        pipelineStub2.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub2.AfterExecutionTime.Should().NotBeNull();
        pipelineStub3.BeforeExecutionTime.Should().NotBeNull();
        pipelineStub3.AfterExecutionTime.Should().NotBeNull();
    }
}
