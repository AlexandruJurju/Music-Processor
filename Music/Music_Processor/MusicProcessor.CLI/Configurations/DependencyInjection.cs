using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MusicProcessor.CLI.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCli(this IServiceCollection services)
    {
        RegisterCliConfiguration(services);

        return services;
    }

    private static void RegisterCliConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider => new CliApplicationBuilder()
                .SetTitle("Music Processor CLI application")
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(type => ActivatorUtilities.CreateInstance(serviceProvider, type))
                .Build());

        services.AddScoped<InteractiveCli>();
    }
}
