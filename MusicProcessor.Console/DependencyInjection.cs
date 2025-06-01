using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MusicProcessor.Console;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        AddCliCommands(services);

        return services;
    }

    private static void AddCliCommands(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider => new CliApplicationBuilder()
            .SetTitle("Music Processor CLI application")
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(type => ActivatorUtilities.CreateInstance(serviceProvider, type))
            .Build());

        services.AddScoped<InteractiveCli>();
    }
}
