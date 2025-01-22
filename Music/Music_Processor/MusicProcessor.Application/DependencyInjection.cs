using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Services;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient<ISongProcessor, SongProcessor>();
        services.AddTransient<IMetadataStrategy, FlacMetadataHandler>();
        services.AddTransient<IMetadataStrategy, MP3MetadataHandler>();
        services.AddTransient<MetadataService>();

        return services;
    }
}