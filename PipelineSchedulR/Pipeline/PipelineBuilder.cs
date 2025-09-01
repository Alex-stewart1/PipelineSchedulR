using Microsoft.Extensions.DependencyInjection;
using PipelineSchedulR.Common.Helpers;
using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Pipeline;

public interface IPipelineBuilder
{
    IPipelineExecutable Executable<TExecutable>() where TExecutable : IExecutable;
}
public interface IPipelineExecutable
{
    IPipelineExecutable WithPipeline<TPipeline>() where TPipeline : IPipeline;
}
public class PipelineBuilder(IServiceCollection services) : IPipelineBuilder, IPipelineExecutable
{
    private readonly IServiceCollection _services = services;
    private Type _executableType = null!;

    public IPipelineExecutable Executable<TExecutable>() where TExecutable : IExecutable
    {
        _executableType = typeof(TExecutable);

        _services.AddKeyedScoped(serviceType: typeof(IExecutable),
                                 serviceKey: KeyedServiceHelper.GetExecutableKey(_executableType),
                                 (provider, key) => provider.GetRequiredService<TExecutable>());
        return this;
    }

    public IPipelineExecutable WithPipeline<TPipeline>() where TPipeline : IPipeline
    {
        _services.AddKeyedScoped(serviceType: typeof(IPipeline),
                                 serviceKey: KeyedServiceHelper.GetExecutableKey(_executableType),
                                 (provider, key) => provider.GetRequiredService<TPipeline>());
        return this;
    }
}
