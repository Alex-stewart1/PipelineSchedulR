using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchedulR.Pipeline;
using SchedulR.Scheduling;
using SchedulR.Scheduling.Configuration;
using SchedulR.Scheduling.Interfaces;


namespace SchedulR.Common.Registration;

internal static class SchedulRRegistrationExtensions
{
    public static IServiceCollection AddSchedulR(this IServiceCollection services, Action<IPipelineBuilder, SchedulerOptions> configure)
    {
        var options = new SchedulerOptions();
        var builder = new PipelineBuilder(services);

        configure(builder, options);

        services.AddSingleton(provider => new Scheduler(provider.GetRequiredService<IServiceScopeFactory>(),
                                                        options,
                                                        provider.GetService<ILogger<Scheduler>>()));

        return services;
    }

    public static IServiceProvider UseSchedulR(this IServiceProvider provider, Action<IScheduler> configureScheduler)
    {
        var scheduler = provider.GetRequiredService<Scheduler>();

        configureScheduler(scheduler);

        return provider;
    }
}
