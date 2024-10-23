using SchedulR.Interfaces;

namespace SchedulR.Tests.Stubs.Pipeline;

internal class ExecutableStub : IExecutable<string>
{
    public Task<string> ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(nameof(ExecutableStub));
    }
}
