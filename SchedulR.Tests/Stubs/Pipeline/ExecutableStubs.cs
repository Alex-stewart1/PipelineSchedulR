using SchedulR.Common.Types;
using SchedulR.Interfaces;

namespace SchedulR.Tests.Stubs.Pipeline;

internal class BaseExecutableStub : IExecutable
{
    public DateTimeOffset? ExecutionTime { get; private set; } = null;
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(100, CancellationToken.None); // Simulate some work

        ExecutionTime = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}

internal class ExecutableStub1 : BaseExecutableStub { }
internal class ExecutableStub2 : BaseExecutableStub { }