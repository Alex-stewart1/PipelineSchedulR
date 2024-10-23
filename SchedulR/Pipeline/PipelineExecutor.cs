using SchedulR.Interfaces;
using SchedulR.Pipeline.Executors;

namespace SchedulR.Pipeline;

internal class PipelineExecutor
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResult> ExecuteAsync<TResult, TExecutable>(CancellationToken cancellationToken) where TExecutable : IExecutable<TResult>
        => ExecuteAsync<TResult>(typeof(TExecutable), cancellationToken);

    private async Task<TResult> ExecuteAsync<TResult>(Type executorType, CancellationToken cancellationToken)
    {
        var pipelineExecutorType = typeof(PipelineExecutor).MakeGenericType(executorType);
        var pipelineExecutor = (BasePipelineExecutor)(Activator.CreateInstance(pipelineExecutorType, _serviceProvider) ?? throw new InvalidOperationException("Unable to create instance of pipeline executor."));
        var result = await pipelineExecutor.BaseExecuteAsync(_serviceProvider, cancellationToken);
        return result is null ? throw new InvalidOperationException("Execution result is null.") : (TResult)result;
    }
}
