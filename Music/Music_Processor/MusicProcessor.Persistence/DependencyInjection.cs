using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Persistence.Albums;
using MusicProcessor.Persistence.Artists;
using MusicProcessor.Persistence.Common;
using MusicProcessor.Persistence.GenreCategories;
using MusicProcessor.Persistence.Genres;
using MusicProcessor.Persistence.SongsMetadata;

namespace MusicProcessor.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection RegisterPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        AddDb(services, configuration);
        AddRepositories(services);

        return services;
    }

    private static void AddDb(IServiceCollection services, IConfiguration configuration)
    {
        var dbPath = configuration.GetValue<string>("PathsSettings:SQLLiteConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
            options.UseSnakeCaseNamingConvention();
        });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddTransient<ISongMetadataRepository, SongMetadataRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IGenreCategoryRepository, GenreCategoryRepository>();
        services.AddTransient<IArtistRepository, ArtistRepository>();
        services.AddTransient<IAlbumRepository, AlbumRepository>();
    }
}