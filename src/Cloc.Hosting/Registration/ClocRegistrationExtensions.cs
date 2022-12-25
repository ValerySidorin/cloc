using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Cloc.Hosting;

public static class ClocRegistrationExtensions
{
    public static IServiceCollection AddClocJob<TClocJob>(
        this IServiceCollection services)
        where TClocJob : class, IClocJob
    {
        if (!services.Any(s => s.ServiceType == typeof(TClocJob)))
        {
            if (typeof(TClocJob).BaseType == typeof(ClocJob))
            {
                services.AddSingleton(typeof(ClocJob), typeof(TClocJob));
            }
            else if (typeof(TClocJob).BaseType == typeof(ScopedClocJob))
            {
                services.AddScoped(typeof(ScopedClocJob), typeof(TClocJob));
            }
        }

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

        Debug.Assert(configuration is not null);

        if (configureOptions.WithSingletonJobs)
        {
            if (!services.Any(s => s.ServiceType == typeof(ClocBackgroundService)))
            {
                services.AddHostedService(sp =>
                {
                    var jobs = sp.GetServices<ClocJob>();
                    var options = GetClocOptions(configuration, configureOptions.ConfigSectionName);
                    var scheduler = new ClocScheduler(options, jobs);
                    return new ClocBackgroundService(scheduler);
                });
            }
        }

        if (configureOptions.WithScopedJobs)
        {
            if (!services.Any(s => s.ServiceType == typeof(ScopedClocBackgroundService)))
            {
                services.AddHostedService(sp =>
                {
                    var options = GetClocOptions(configuration, configureOptions.ConfigSectionName);
                    var scheduler = new ScopedClocScheduler(options, sp);
                    return new ScopedClocBackgroundService(scheduler);
                });
            }
        }

        return services;
    }

    public static IServiceCollection AddCloc<TClocJob>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AddClocBackgroundServiceOptions> configure = null)
        where TClocJob : class, IClocJob
    {
        services.AddClocJob<TClocJob>();
        services.AddCloc(configuration, configure);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AddClocBackgroundServiceOptions> configure = null)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddCloc(configuration, configure);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2, TClocJob3>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AddClocBackgroundServiceOptions> configure = null)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
        where TClocJob3: class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddClocJob<TClocJob3>();
        services.AddCloc(configuration, configure);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2, TClocJob3, TClocJob4>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AddClocBackgroundServiceOptions> configure = null)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
        where TClocJob3 : class, IClocJob
        where TClocJob4 : class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddClocJob<TClocJob3>();
        services.AddClocJob<TClocJob4>();
        services.AddCloc(configuration, configure);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2, TClocJob3, TClocJob4, TClocJob5>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AddClocBackgroundServiceOptions> configure = null)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
        where TClocJob3 : class, IClocJob
        where TClocJob4 : class, IClocJob
        where TClocJob5 : class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddClocJob<TClocJob3>();
        services.AddClocJob<TClocJob4>();
        services.AddClocJob<TClocJob5>();
        services.AddCloc(configuration, configure);
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
