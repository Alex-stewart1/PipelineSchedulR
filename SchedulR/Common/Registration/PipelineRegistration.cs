using Microsoft.Extensions.DependencyInjection;
using SchedulR.Pipeline;

namespace SchedulR.Common.Registration;

internal static class PipelineRegistration
{
    public static IServiceCollection AddSchedulR(this IServiceCollection services, Action<IPipelineBuilder> pipelineBuilder)
    {
        var builder = new PipelineBuilder(services);
        pipelineBuilder(builder);
        return services;
    }
}
