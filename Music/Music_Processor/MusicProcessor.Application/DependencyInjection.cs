using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Factories;
using MusicProcessor.Application.Services;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        services.AddTransient<IMetadataService, MetadataService>();
        services.AddTransient<IPlaylistProcessor, PlaylistProcessor>();
        services.AddSingleton<MetadataHandlerFactory>();

        return services;
    }
}