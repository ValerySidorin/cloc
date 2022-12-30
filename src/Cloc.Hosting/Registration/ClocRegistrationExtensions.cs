using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace Cloc.Hosting;

public static class ClocRegistrationExtensions
{
    private static IServiceCollection AddClocJob(
        this IServiceCollection services, 
        Type type)
    {
        if (type.BaseType == typeof(ClocJob))
        {
            services.AddSingleton(typeof(ClocJob), type);
        }
        else if (type.BaseType == typeof(ScopedClocJob))
        {
            services.AddScoped(typeof(ScopedClocJob), type);
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

        var types = Assembly.GetCallingAssembly()
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Append(Assembly.GetCallingAssembly())
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IClocJob).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .ToList();

        foreach (var type in types)
        {
            services.AddClocJob(type);
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

    private static ClocOptions GetClocOptions(IConfiguration configuration, string sectionName)
    {
        var options = new ClocOptions();
        configuration.GetSection(sectionName)
            .Bind(options);
        return options;
    }
}
