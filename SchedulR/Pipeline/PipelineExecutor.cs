using Microsoft.Extensions.DependencyInjection;
using SchedulR.Common.Helpers;
using SchedulR.Common.Types;
using SchedulR.Interfaces;

namespace SchedulR.Pipeline;

internal static class PipelineExecutor
{
    /// <summary>
    /// Executes the pipeline associated with the executor type.
    /// </summary>
    /// <param name="executorType"></param>
    /// <param name="provider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Result of the pipeline execution
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static Task<Result> ExecuteAsync(Type executorType, IServiceProvider provider, CancellationToken cancellationToken)
    {
        // Ensure the executor type implements IExecutable
        if (!typeof(IExecutable).IsAssignableFrom(executorType))
        {
            throw new InvalidOperationException($"Type {executorType.Name} does not implement {typeof(IExecutable).FullName}");
        }

        var executableKey = KeyedServiceHelper.GetExecutableKey(executorType);

        PipelineDelegate current = (ct) => provider.GetRequiredKeyedService<IExecutable>(executableKey).ExecuteAsync(ct);

        var pipelines = provider.GetKeyedServices<IPipeline>(executableKey);

        foreach (var pipeline in pipelines.Reverse())
        {
            PipelineDelegate next = current;
            current = (ct) => pipeline.ExecuteAsync(next, ct);
        }

        return current(cancellationToken);
    }
}
