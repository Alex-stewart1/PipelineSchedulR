using Microsoft.Extensions.DependencyInjection;
using SchedulR.Interfaces;

namespace SchedulR.Pipeline;

internal class PipelineExecutor
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal Task<TResult> ExecuteAsync<TResult>(Type executorType, CancellationToken cancellationToken)
    {
        //TODO: Implement proper key (guid + some name)
        var executableKey = executorType.FullName ?? throw new InvalidOperationException($"Failed to get key for {executorType.Name}");

        PipelineDelegate<TResult> current = (ct) => _serviceProvider.GetRequiredKeyedService<IExecutable<TResult>>(executableKey).ExecuteAsync(ct);

        var pipelines = _serviceProvider.GetKeyedServices<IPipeline<TResult>>(executableKey);

        foreach (var pipeline in pipelines.Reverse())
        {
            PipelineDelegate<TResult> next = current;
            current = (ct) => pipeline.ExecuteAsync(next, cancellationToken);
        }

        return current(cancellationToken);

    }
}
