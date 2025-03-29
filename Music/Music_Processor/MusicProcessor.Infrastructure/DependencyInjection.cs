using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Infrastructure.FileService;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PathsOptions>(configuration.GetSection("PathsSettings"));

        AddExternalFileServices(services);

        return services;
    }

    private static void AddExternalFileServices(IServiceCollection services)
    {
        services.AddTransient<IFileService, PhysicalFileService>();
    }
}