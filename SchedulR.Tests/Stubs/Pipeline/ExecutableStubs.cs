using SchedulR.Interfaces;

namespace SchedulR.Tests.Stubs.Pipeline;

internal class ExecutableStub1 : IExecutable<string>
{
    public DateTimeOffset? ExecutionTime { get; private set; } = null;
    public string? Result { get; private set; } = null;
    public async Task<string> ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(100, CancellationToken.None); // Simulate some work

        ExecutionTime = DateTimeOffset.UtcNow;
        Result = nameof(ExecutableStub1);

        return Result;
    }
}
internal class ExecutableStub2 : IExecutable<string>
{
    public DateTimeOffset? ExecutionTime { get; private set; } = null;
    public string? Result { get; private set; } = null;
    public async Task<string> ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(100, CancellationToken.None); // Simulate some work

        ExecutionTime = DateTimeOffset.UtcNow;
        Result = nameof(ExecutableStub2);

        return Result;
    }
}
