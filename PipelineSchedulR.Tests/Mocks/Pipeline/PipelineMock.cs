using PipelineSchedulR.Common.Types;
using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Tests.Mocks.Pipeline;
internal class BasePipelineMock : IPipeline
{
    public DateTimeOffset? BeforeExecutionTime { get; private set; } = null;
    public DateTimeOffset? AfterExecutionTime { get; private set; } = null;
    public Type? ExecutableType { get; private set; } = null;
    public async Task<Result> ExecuteAsync(PipelineDelegate next, Type executableType, CancellationToken cancellationToken)
    {
        BeforeExecutionTime = DateTimeOffset.UtcNow;
        
        ExecutableType = executableType;

        var result = await next(cancellationToken);

        AfterExecutionTime = DateTimeOffset.UtcNow;

        return result;
    }
}
internal class PipelineMock1 : BasePipelineMock { }
internal class PipelineMock2 : BasePipelineMock { }
internal class PipelineMock3 : BasePipelineMock { }
