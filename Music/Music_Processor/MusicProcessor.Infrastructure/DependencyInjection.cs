using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Infrastructure.DataSyncService;
using MusicProcessor.Infrastructure.Persistence;
using MusicProcessor.Infrastructure.Persistence.Repositories;
using MusicProcessor.Infrastructure.SpotDL;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services)
    {
        AddExternalFileServices(services);
        AddDb(services);
        AddRepositories(services);

        return services;
    }

    private static void AddExternalFileServices(IServiceCollection services)
    {
        services.AddTransient<ISpotDLService, SpotDLService>();
        services.AddTransient<IFileService, FileService.FileService>();
        services.AddTransient<ISpotDLMetadataLoader, SpotDLMetadataLoader>();
        services.AddTransient<IGenreSyncService, GenreSyncService>();
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
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IGenreCategoryRepository, GenreCategoryRepository>();
        services.AddTransient<IArtistRepository, ArtistRepository>();
        services.AddTransient<IAlbumRepository, AlbumRepository>();
    }
}