using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Services;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        RegisterMediatr(services);

        RegisterServices(services);

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IMetadatImportService, MetadataImportService>();
        services.AddTransient<IMetadataService, MetadataService>();
    }

    private static void RegisterMediatr(IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); });
    }
}
