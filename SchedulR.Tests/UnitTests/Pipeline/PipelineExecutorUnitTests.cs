using Microsoft.Extensions.DependencyInjection;
using SchedulR.Interfaces;
using SchedulR.Pipeline;
using SchedulR.Tests.Stubs.Pipeline;

namespace SchedulR.Tests.UnitTests.Pipeline;

public class PipelineExecutorUnitTests
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineExecutorUnitTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddScoped<IExecutable<string>, ExecutableStub>()
            .AddScoped<IPipeline<string, ExecutableStub>, PipelineStubs>()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCalledWithExecutableType_ShouldReturnResult()
    {
        // Arrange
        var pipelineExecutor = new PipelineExecutor(_serviceProvider);
        // Act
        var result = await pipelineExecutor.ExecuteAsync<string, ExecutableStub>(CancellationToken.None);

        // Assert
        Assert.Equal(nameof(ExecutableStub), result);
    }
}
