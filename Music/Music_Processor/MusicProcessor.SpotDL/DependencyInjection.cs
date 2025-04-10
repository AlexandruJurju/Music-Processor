using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.SpotDL.Interfaces;
using MusicProcessor.SpotDL.SpotDL;

namespace MusicProcessor.SpotDL;

public static class DependencyInjection
{
    public static IServiceCollection RegisterSpotDL(this IServiceCollection services)
    {
        services.AddScoped<ISpotDLMetadataReader, SpotDLMetadataReader>();
        return services;
    }
}
