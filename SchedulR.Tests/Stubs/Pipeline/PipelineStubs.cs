using SchedulR.Interfaces;

namespace SchedulR.Tests.Stubs.Pipeline;

internal class PipelineStubs : IPipeline<string, ExecutableStub>
{
    public async Task<string> ExecuteAsync(PipelineDelegate<string> next, CancellationToken cancellationToken)
    {
        return await next(cancellationToken);
    }
}
