using PipelineSchedulR.Common.Types;

namespace PipelineSchedulR.Interfaces;

/// <summary>
/// Represents a task that can be executed by the scheduler pipeline
/// </summary>
public interface IExecutable
{
    Task<Result> ExecuteAsync(CancellationToken cancellationToken);
}
