using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.CLI.MenuOptions;
using MusicProcessor.CLI.MenuOptions.Abstractions;

namespace MusicProcessor.CLI;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCLI(this IServiceCollection services)
    {
        services.AddTransient<IMenuOption, FirstTimeSyncOption>();
        services.AddTransient<IMenuOption, UpdateSyncOption>();
        services.AddTransient<IMenuOption, FixPlaylistMetadataOption>();
        services.AddTransient<IMenuOption, ExitOption>();
        services.AddTransient<IMenuOption, TestWriteToDBWithSpotdlMeta>();
        services.AddTransient<IMenuOption, CommitChangesToLibraryOption>();

        services.AddScoped<CommandLine>();
        services.AddScoped<MenuCommandFactory>();

        return services;
    }
}