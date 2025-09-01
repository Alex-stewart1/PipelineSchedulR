using NSubstitute;
using PipelineSchedulR.Interfaces;
using PipelineSchedulR.Pipeline;

namespace PipelineSchedulR.Tests.UnitTests.Pipeline;
public class PipelineExecutorTests
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineExecutorTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowInvalidOperationException_WhenExecutorTypeDoesNotImplementIExecutable()
    {
        // Arrange
        var invalidType = typeof(string); // string does not implement IExecutable

        // Act
        Func<Task> act = async () => await PipelineExecutor.ExecuteAsync(invalidType, _serviceProvider, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Type {invalidType.Name} does not implement {typeof(IExecutable).FullName}");
    }
}

