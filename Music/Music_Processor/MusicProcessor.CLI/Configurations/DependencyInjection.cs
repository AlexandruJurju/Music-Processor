using CliFx;
using Microsoft.Extensions.DependencyInjection;

namespace MusicProcessor.CLI.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCLI(this IServiceCollection services)
    {
        RegisterCliConfiguration(services);
        
        return services;
    }

    private static void RegisterCliConfiguration(IServiceCollection services)
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
}