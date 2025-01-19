using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.CLI.Commands;

namespace MusicProcessor.CLI.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection RegisterCLI(this IServiceCollection services)
    {
        services.AddScoped<FixMetadataCommand>();
        services.AddScoped<CommitChangesCommand>();
        services.AddScoped<FirstTimeSyncCommand>();
        services.AddScoped<UpdateSyncCommand>();
        services.AddScoped<CommandLineApp>();
        return services;
    }
}