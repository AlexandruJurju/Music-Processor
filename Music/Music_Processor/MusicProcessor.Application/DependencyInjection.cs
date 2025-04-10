using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Services;
using Wolverine;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        RegisterWolverine(services);

        RegisterServices(services);

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IMetadatImportService, MetadataImportService>();
        services.AddTransient<IMetadataService, MetadataService>();
    }

    private static void RegisterWolverine(IServiceCollection services)
    {
        services.AddWolverine(opts =>
        {
            // Automatically find and register message handlers from the assembly
            opts.Discovery.IncludeAssembly(Assembly.GetExecutingAssembly());
        });
    }
}