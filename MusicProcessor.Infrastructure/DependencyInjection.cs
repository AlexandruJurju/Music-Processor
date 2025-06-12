using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Infrastructure.Repositories;
using Raven.Client.Documents;
using Raven.Embedded;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRavenDb(services, configuration);

        AddRepositories(services);

        AddHealthChecks(services);

        AddServices(services);

        return services;
    }


    private static void AddHealthChecks(IServiceCollection services)
    {
        services
            .AddHealthChecks();
    }


    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ISongRepository, SongRepository>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IMetadataService, MetadataService.MetadataService>();
        services.AddScoped<IFileService, FileService.FileService>();
        services.AddScoped<IAudioService, AudioService.AudioService>();
    }

    private static void AddRavenDb(IServiceCollection services, IConfiguration configuration)
    {
        EmbeddedServer.Instance.StartServer(new ServerOptions
        {
            DataDirectory = configuration["RavenDB:DataDirectory"]!,
            ServerUrl = configuration["RavenDB:ServerUrl"]!
        });

        services.AddSingleton<IDocumentStore>(_ =>
        {
            IDocumentStore store = EmbeddedServer.Instance.GetDocumentStore(configuration["RavenDB:DatabaseName"]!);
            return store;
        });
    }
}
