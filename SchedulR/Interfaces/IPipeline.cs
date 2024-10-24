using SchedulR.Common.Types;

namespace SchedulR.Interfaces;

public delegate Task<Result> PipelineDelegate(CancellationToken cancellationToken);

public interface IPipeline
{
    Task<Result> ExecuteAsync(PipelineDelegate next, CancellationToken cancellationToken);
}
