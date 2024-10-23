namespace SchedulR.Interfaces;

/// <summary>
/// Represents a task that can be executed by the scheduler pipeline
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IExecutable<TResult>
{
    Task<TResult> ExecuteAsync(CancellationToken cancellationToken);
}
