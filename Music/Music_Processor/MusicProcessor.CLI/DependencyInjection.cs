using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.CLI.MenuCommands;

namespace MusicProcessor.CLI;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCLI(this IServiceCollection services)
    {
        services.AddTransient<IMenuCommand, FirstTimeSyncCommand>();
        services.AddTransient<IMenuCommand, UpdateSyncCommand>();
        services.AddTransient<IMenuCommand, FixGenresSpotDLCommand>();
        services.AddTransient<IMenuCommand, FixGenresCustomMetadata>();
        services.AddTransient<IMenuCommand, ExitCommand>();
        services.AddTransient<IMenuCommand, SyncDbCommand>();

        services.AddScoped<CommandLine>();
        services.AddScoped<MenuCommandFactory>();

        return services;
    }
}