using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Infrastructure.Database;
using MusicProcessor.Infrastructure.Repositories;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddSqlite(services, configuration);

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

    private static void AddSqlite(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("SQLite")!;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={connectionString}");
            options.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IStyleRepository, StyleRepository>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ISpotDLMetadataReader, SpotDLMetadataReader.SpotDLMetadataReader>();
        services.AddScoped<IFileService, FileService.FileService>();
        services.AddScoped<IMetadataService, MetadataService.MetadataService>();
    }
}
