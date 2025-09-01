using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipelineSchedulR.Pipeline;
using PipelineSchedulR.Scheduling;
using PipelineSchedulR.Scheduling.Configuration;
using PipelineSchedulR.Scheduling.Interfaces;


namespace PipelineSchedulR.Common.Registration;

public static class SchedulRRegistrationExtensions
{
    public static IServiceCollection AddSchedulR(this IServiceCollection services, Action<IPipelineBuilder, SchedulerOptions> configure)
    {
        var options = new SchedulerOptions();
        var builder = new PipelineBuilder(services);

        configure(builder, options);

        services.AddHostedService<SchedulerHost>();
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
