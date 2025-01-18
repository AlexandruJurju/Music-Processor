using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.CLI.MenuCommands;

namespace MusicProcessor.CLI;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCLI(this IServiceCollection services)
    {
        services.AddTransient<IMenuOption, FirstTimeSyncOption>();
        services.AddTransient<IMenuOption, UpdateSyncOption>();
        services.AddTransient<IMenuOption, SyncDbMenuOption>();
        services.AddTransient<IMenuOption, ExitOption>();

        services.AddScoped<CommandLine>();
        services.AddScoped<MenuCommandFactory>();

        return services;
    }
}