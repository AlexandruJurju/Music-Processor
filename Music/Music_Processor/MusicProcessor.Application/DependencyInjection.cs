using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Services;
using MusicProcessor.Application.Services.MetadataHandlerFactory;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        RegisterMediatR(services);

        RegisterServices(services);

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<ISongProcessor, SongProcessor>();
        services.AddTransient<IMetadataService, MetadataService>();
        
        services.AddTransient<IMetadataHandler, MP3MetadataHandler>();
        services.AddTransient<IMetadataHandler, FlacMetadataHandler>();
    }

    private static void RegisterMediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}