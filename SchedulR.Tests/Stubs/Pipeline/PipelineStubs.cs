using SchedulR.Interfaces;

namespace SchedulR.Tests.Stubs.Pipeline;

internal class PipelineStub1 : IPipeline<string>
{
    public DateTimeOffset? BeforeExecutionTime { get; private set; } = null;
    public DateTimeOffset? AfterExecutionTime { get; private set; } = null;
    public string? Result { get; private set; } = null;
    public async Task<string> ExecuteAsync(PipelineDelegate<string> next, CancellationToken cancellationToken)
    {
        BeforeExecutionTime = DateTimeOffset.UtcNow;

        Result = await next(cancellationToken);

        AfterExecutionTime = DateTimeOffset.UtcNow;

        return Result;
    }
}
internal class PipelineStub2 : IPipeline<string>
{
    public DateTimeOffset? BeforeExecutionTime { get; private set; } = null;
    public DateTimeOffset? AfterExecutionTime { get; private set; } = null;
    public string? Result { get; private set; } = null;
    public async Task<string> ExecuteAsync(PipelineDelegate<string> next, CancellationToken cancellationToken)
    {
        BeforeExecutionTime = DateTimeOffset.UtcNow;

        Result = await next(cancellationToken);

        AfterExecutionTime = DateTimeOffset.UtcNow;

        return Result;
    }
}
internal class PipelineStub3 : IPipeline<string>
{
    public DateTimeOffset? BeforeExecutionTime { get; private set; } = null;
    public DateTimeOffset? AfterExecutionTime { get; private set; } = null;
    public string? Result { get; private set; } = null;
    public async Task<string> ExecuteAsync(PipelineDelegate<string> next, CancellationToken cancellationToken)
    {
        BeforeExecutionTime = DateTimeOffset.UtcNow;

        Result = await next(cancellationToken);

        AfterExecutionTime = DateTimeOffset.UtcNow;

        return Result;
    }
}
