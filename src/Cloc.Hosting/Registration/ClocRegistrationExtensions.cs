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
        if (typeof(TClocJob).BaseType == typeof(ClocJob))
        {
            services.AddSingleton(typeof(ClocJob), typeof(TClocJob));
        }
        else if (typeof(TClocJob).BaseType == typeof(ScopedClocJob))
        {
            services.AddScoped(typeof(ScopedClocJob), typeof(TClocJob));
        }

        return services;
    }

    public static IServiceCollection AddCloc(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ClocOptions> configure = null,
        string configSectionName = ClocOptions.DefaultSectionName)
    {
        Debug.Assert(configuration is not null);

        var options = GetClocOptions(configuration, configSectionName);
        if (configure is not null)
        {
            configure(options);
        }

        if (!services.Any(s => s.ServiceType == typeof(ClocBackgroundService)))
        {
            services.AddHostedService(sp =>
            {
                var scheduler = new ClocScheduler(options.ExitOnJobFailed);
                return new ClocBackgroundService(scheduler, options.Jobs, sp);
            });
        }

        return services;
    }

    public static IServiceCollection AddCloc<TClocJob>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ClocOptions> configure = null,
        string configSectionName = ClocOptions.DefaultSectionName)
        where TClocJob : class, IClocJob
    {
        services.AddClocJob<TClocJob>();
        services.AddCloc(configuration, configure, configSectionName);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ClocOptions> configure = null,
        string configSectionName = ClocOptions.DefaultSectionName)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddCloc(configuration, configure, configSectionName);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2, TClocJob3>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ClocOptions> configure = null,
        string configSectionName = ClocOptions.DefaultSectionName)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
        where TClocJob3: class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddClocJob<TClocJob3>();
        services.AddCloc(configuration, configure, configSectionName);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2, TClocJob3, TClocJob4>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ClocOptions> configure = null,
        string configSectionName = ClocOptions.DefaultSectionName)
        where TClocJob1 : class, IClocJob
        where TClocJob2 : class, IClocJob
        where TClocJob3 : class, IClocJob
        where TClocJob4 : class, IClocJob
    {
        services.AddClocJob<TClocJob1>();
        services.AddClocJob<TClocJob2>();
        services.AddClocJob<TClocJob3>();
        services.AddClocJob<TClocJob4>();
        services.AddCloc(configuration, configure, configSectionName);
        return services;
    }

    public static IServiceCollection AddCloc<TClocJob1, TClocJob2, TClocJob3, TClocJob4, TClocJob5>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ClocOptions> configure = null, 
        string configSectionName = ClocOptions.DefaultSectionName)
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
        services.AddCloc(configuration, configure, configSectionName);
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
