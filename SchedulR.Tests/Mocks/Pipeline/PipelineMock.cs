using SchedulR.Common.Types;
using SchedulR.Interfaces;

namespace SchedulR.Tests.Mocks.Pipeline;
internal class BasePipelineMock : IPipeline
{
    public DateTimeOffset? BeforeExecutionTime { get; private set; } = null;
    public DateTimeOffset? AfterExecutionTime { get; private set; } = null;
    public async Task<Result> ExecuteAsync(PipelineDelegate next, CancellationToken cancellationToken)
    {
        BeforeExecutionTime = DateTimeOffset.UtcNow;

        var result = await next(cancellationToken);

        AfterExecutionTime = DateTimeOffset.UtcNow;

        return result;
    }
}
internal class PipelineMock1 : BasePipelineMock { }
internal class PipelineMock2 : BasePipelineMock { }
internal class PipelineMock3 : BasePipelineMock { }
