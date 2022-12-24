using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cloc.Hosting;

public static class ClocRegistrationExtensions
{
    public static IServiceCollection AddClocJob<TClocJob>(
        this IServiceCollection services)
        where TClocJob : ClocJob
    {
        services.AddSingleton<ClocJob, TClocJob>();
        return services;
    }

    public static IServiceCollection AddScopedClocJob<TScopedClocJob>(
        this IServiceCollection services)
        where TScopedClocJob : ScopedClocJob
    {
        services.AddScoped<ScopedClocJob, TScopedClocJob>();
        return services;
    }

    public static IServiceCollection AddCloc(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AddClocBackgroundServiceOptions> configure = null)
    {
        var configureOptions = new AddClocBackgroundServiceOptions();
        if (configure is not null)
        {
            configure(configureOptions);
        }

        EnsureArg.IsNotNull(configuration, nameof(configuration));

        if (configureOptions.WithSingletonJobs)
        {
            services.AddHostedService(sp =>
            {
                var jobs = sp.GetServices<ClocJob>();
                var options = GetClocOptions(configuration, configureOptions.ConfigSectionName);
                var scheduler = new ClocScheduler(options, jobs);
                return new ClocBackgroundService(scheduler);
            });
        }

        if (configureOptions.WithScopedJobs)
        {
            services.AddHostedService(sp =>
            {
                var options = GetClocOptions(configuration, configureOptions.ConfigSectionName);
                var scheduler = new ScopedClocScheduler(options, sp);
                return new ScopedClocBackgroundService(scheduler);
            });
        }

        return services;
    }

    private static ClocOptions GetClocOptions(IConfiguration configuration, string sectionName)
    {
        var options = new ClocOptions();
        configuration.GetSection(sectionName)
            .Bind(options);
        return options;
    }
}
