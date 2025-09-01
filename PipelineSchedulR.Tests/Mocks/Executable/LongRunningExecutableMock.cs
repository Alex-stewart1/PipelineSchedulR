using PipelineSchedulR.Common.Types;
using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Tests.Mocks.Executable;

/// <summary>
/// Mock for an executable that runs for a simulated long time and can be cancelled with either the passed cancellation token or the scheduler token
/// 
/// <para>
/// Note the execution will be cancelled after 100 iterations (100 seconds) or when the passed cancellation token is cancelled
/// </para>
/// </summary>
/// <param name="cancellationToken"></param>
internal class LongRunningExecutableMock(CancellationToken cancellationToken) : IExecutable
{
    public CancellationToken CancellationToken { get; } = cancellationToken;
    public int ExecutionCount { get; private set; } = 0;

    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken)
    {
        ExecutionCount++;

        var count = 0;

        while (!CancellationToken.IsCancellationRequested)
        {
            if (count++ > 100) // Prevent test from running indefinitely
            {
                throw new InvalidOperationException("Test timed out");
            }

            try
            {
                await Task.Delay(1000, CancellationToken); // Simulate some work
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        return Result.Success();
    }
}