using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MusicProcessor.CLI.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCLI(this IServiceCollection services)
    {
        RegisterCliConfiguration(services);
        RegisterAppSettings(services);

        return services;
    }

    private static void RegisterCliConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            return new CliApplicationBuilder()
                .SetTitle("Music Processor CLI application")
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(type => ActivatorUtilities.CreateInstance(serviceProvider, type))
                .Build();
        });

        services.AddScoped<InteractiveCli>();
    }

    private static void RegisterAppSettings(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }
}