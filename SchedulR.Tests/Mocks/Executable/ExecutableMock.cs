using SchedulR.Common.Types;
using SchedulR.Interfaces;

namespace SchedulR.Tests.Mocks.Executable;

internal class BaseExecutableMock : IExecutable
{
    public List<DateTimeOffset> ExecutionTimes { get; } = [];
    public bool CancellationWasRequested { get; private set; } = false;
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(100, cancellationToken); // Simulate some work

            ExecutionTimes.Add(DateTimeOffset.UtcNow);
        }
        catch (OperationCanceledException)
        {
            CancellationWasRequested = true;
        }


        return Result.Success();
    }
}

internal class ExecutableMock1 : BaseExecutableMock { }
internal class ExecutableMock2 : BaseExecutableMock { }
