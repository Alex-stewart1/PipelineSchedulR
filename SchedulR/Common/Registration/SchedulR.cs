using Microsoft.Extensions.DependencyInjection;
using SchedulR.Pipeline;
using SchedulR.Scheduling;
using SchedulR.Scheduling.Interfaces;


namespace SchedulR.Common.Registration;

internal static class SchedulRRegistrationExtensions
{
    public static IServiceCollection AddSchedulR(this IServiceCollection services, Action<IPipelineBuilder> pipelineBuilder)
    {
        services.AddSingleton<Scheduler>();

        var builder = new PipelineBuilder(services);
        pipelineBuilder(builder);
        return services;
    }

    public static IServiceProvider UseSchedulR(this IServiceProvider provider, Action<IScheduler> configureScheduler)
    {
        var scheduler = provider.GetRequiredService<Scheduler>();

        configureScheduler(scheduler);

        return provider;
    }
}
