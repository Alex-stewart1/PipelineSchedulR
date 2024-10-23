namespace SchedulR.Interfaces;

public delegate Task<TResult> PipelineDelegate<TResult>(CancellationToken cancellationToken);

public interface IPipeline<TResult>
{
    Task<TResult> ExecuteAsync(PipelineDelegate<TResult> next, CancellationToken cancellationToken);
}
