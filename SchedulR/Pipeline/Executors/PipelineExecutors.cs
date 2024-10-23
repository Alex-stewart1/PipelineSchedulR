using Microsoft.Extensions.DependencyInjection;
using SchedulR.Interfaces;

namespace SchedulR.Pipeline.Executors;

// This implementation is heavily inspired by the article "Crossing the generic divider" by Jimmy Bogard.
// That article was seemingly inspired his implementation in MediatR and its use of wrappers to bridge the generics gap
// Reference https://github.com/jbogard/MediatR/blob/master/src/MediatR/Wrappers/RequestHandlerWrapper.cs
// Reference https://www.jimmybogard.com/crossing-the-generics-divide/

internal abstract class BasePipelineExecutor
{
    public abstract Task<object?> BaseExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
internal class PipelineExecutor<TResult> : BasePipelineExecutor
{
    public Task<TResult> ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        PipelineDelegate<TResult> current = (ct) => serviceProvider.GetRequiredService<IExecutable<TResult>>().ExecuteAsync(ct);
        PipelineDelegate<TResult> next;

        var pipelines = serviceProvider.GetServices<IPipeline<TResult>>();

        foreach (var pipeline in pipelines.Reverse())
        {
            next = current;
            current = (ct) => pipeline.ExecuteAsync(next, cancellationToken);
        }

        return current(cancellationToken);
    }

    public override async Task<object?> BaseExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(serviceProvider, cancellationToken);
    }
}
