using SchedulR.Common.Types;
using SchedulR.Interfaces;

namespace SchedulR.Tests.Stubs.Pipeline;
internal class BasePipelineStub : IPipeline
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
internal class PipelineStub1 : BasePipelineStub { }
internal class PipelineStub2 : BasePipelineStub { }
internal class PipelineStub3 : BasePipelineStub { }
