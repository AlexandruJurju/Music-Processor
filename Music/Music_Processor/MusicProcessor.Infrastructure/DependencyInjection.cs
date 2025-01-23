using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Infrastructure.Config;
using MusicProcessor.Infrastructure.FileAccess;
using MusicProcessor.Infrastructure.Persistence;
using MusicProcessor.Infrastructure.Persistence.Repositories;
using MusicProcessor.Infrastructure.SpotDL;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddExternalFileServices(services);
        AddDb(services);
        AddRepositories(services);

        return services;
    }

    private static void AddExternalFileServices(IServiceCollection services)
    {
        services.AddTransient<ISpotDLService, SpotDLService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<ISpotDLMetadataLoader, SpotDLMetadataLoader>();
        services.AddTransient<IConfigRepository, ConfigRepository>();
    }

    private static void AddDb(IServiceCollection services)
    {
        var dbPath = Path.Combine(Environment.CurrentDirectory, "music.sqlite");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
            options.UseSnakeCaseNamingConvention();
        });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddTransient<ISongRepository, SongRepository>();
        services.AddTransient<IStyleRepository, StyleRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IArtistRepository, ArtistRepository>();
        services.AddTransient<IAlbumRepository, AlbumRepository>();
    }
}