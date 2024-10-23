using Microsoft.Extensions.DependencyInjection;
using SchedulR.Interfaces;
using SchedulR.Pipeline;
using SchedulR.Tests.Stubs.Pipeline;

namespace SchedulR.Tests.UnitTests.Pipeline;

public class PipelineExecutorUnitTests
{
    public class ExecuteAsync
    {
        [Fact]
        public async Task WhenCalled_ShouldReturnExecutableResult()
        {
            // Arrange
            var executableKey = typeof(ExecutableStub1).FullName ?? throw new InvalidOperationException($"Failed to get key for {typeof(ExecutableStub1).Name}");
            var executableStub = new ExecutableStub1();
            var pipelineStub = new PipelineStub1();
            var serviceProvider = new ServiceCollection()
                .AddKeyedSingleton<IExecutable<string>>(executableKey, executableStub)
                .AddKeyedSingleton<IPipeline<string>>(executableKey, pipelineStub)
                .BuildServiceProvider();

            var pipelineExecutor = new PipelineExecutor(serviceProvider);

            // Act
            var result = await pipelineExecutor.ExecuteAsync<string>(typeof(ExecutableStub1), CancellationToken.None);

            // Assert
            executableStub.Result.Should().NotBeNull();
            pipelineStub.Result.Should().NotBeNull();
            executableStub.Result.Should().Be(result);
            pipelineStub.Result.Should().Be(result);
        }
        [Fact]
        public async Task WhenCalled_ShouldCorrectlyActivatePipeline()
        {
            // Arrange
            var executableKey = typeof(ExecutableStub1).FullName ?? throw new InvalidOperationException($"Failed to get key for {typeof(ExecutableStub1).Name}");
            var executableStub = new ExecutableStub1();
            var pipelineStub = new PipelineStub1();
            var serviceProvider = new ServiceCollection()
                .AddKeyedSingleton<IExecutable<string>>(executableKey, executableStub)
                .AddKeyedSingleton<IPipeline<string>>(executableKey, pipelineStub)
                .BuildServiceProvider();
            var pipelineExecutor = new PipelineExecutor(serviceProvider);

            // Act
            await pipelineExecutor.ExecuteAsync<string>(typeof(ExecutableStub1), CancellationToken.None);

            // Assert
            pipelineStub.BeforeExecutionTime.Should().NotBeNull();
            pipelineStub.AfterExecutionTime.Should().NotBeNull();
            executableStub.ExecutionTime.Should().NotBeNull();
            pipelineStub.BeforeExecutionTime.Should().BeBefore(executableStub.ExecutionTime!.Value);
            executableStub.ExecutionTime.Should().BeBefore(pipelineStub.AfterExecutionTime!.Value);
        }
        [Fact]
        public async Task WhenCalledWithNoPipeline_ShouldReturnExecutableResult()
        {
            // Arrange
            var executableKey = typeof(ExecutableStub1).FullName ?? throw new InvalidOperationException($"Failed to get key for {typeof(ExecutableStub1).Name}");
            var executableStub = new ExecutableStub1();
            var serviceProvider = new ServiceCollection()
                .AddKeyedSingleton<IExecutable<string>>(executableKey, executableStub)
                .BuildServiceProvider();
            var pipelineExecutor = new PipelineExecutor(serviceProvider);

            // Act
            var result = await pipelineExecutor.ExecuteAsync<string>(typeof(ExecutableStub1), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            executableStub.Result.Should().NotBeNull();
            result.Should().Be(executableStub.Result);
        }
        [Fact]
        public async Task WhenCalledWithMultiplePipelines_ShouldCorrectlyActivatePipelines()
        {
            // Arrange
            var executableKey = typeof(ExecutableStub1).FullName ?? throw new InvalidOperationException($"Failed to get key for {typeof(ExecutableStub1).Name}");
            var executableStub = new ExecutableStub1();
            var pipelineStub1 = new PipelineStub1();
            var pipelineStub2 = new PipelineStub2();
            var pipelineStub3 = new PipelineStub3();

            var serviceProvider = new ServiceCollection()
                .AddKeyedSingleton<IExecutable<string>>(executableKey, executableStub)
                .AddKeyedSingleton<IPipeline<string>>(executableKey, pipelineStub1)
                .AddKeyedSingleton<IPipeline<string>>(executableKey, pipelineStub2)
                .AddKeyedSingleton<IPipeline<string>>(executableKey, pipelineStub3)
                .BuildServiceProvider();

            var pipelineExecutor = new PipelineExecutor(serviceProvider);

            // Act
            var result = await pipelineExecutor.ExecuteAsync<string>(typeof(ExecutableStub1), CancellationToken.None);

            // Assert

            // Executable
            executableStub.Result.Should().NotBeNull();
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
        //[Fact]
        //public async Task WhenCalledWithMultipleRegisteredExecutables_ShouldActivateTheCorrectExecutablesPipeline()
        //{
        //    // Arrange
        //    var executableStub1 = new ExecutableStub1();
        //    var pipelineStub1 = new PipelineStub1();

        //    var executableStub2 = new ExecutableStub2();
        //    var pipelineStub2 = new PipelineStub2();
        //    var pipelineStub3 = new PipelineStub3();

        //    var serviceProvider = new ServiceCollection()
        //        .AddKeyedSingleton<IExecutable<string>>(typeof(ExecutableStub1).FullName, executableStub1)
        //        .AddSingleton<IPipeline<string, ExecutableStub1>>(pipelineStub1)
        //        .AddKeyedSingleton<IExecutable<string>>(typeof(ExecutableStub2).FullName, executableStub2)
        //        .AddSingleton<IPipeline<string, ExecutableStub2>>(pipelineStub2)
        //        .BuildServiceProvider();

        //    var pipelineExecutor = new PipelineExecutor(serviceProvider);

        //}
    }
}
