using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Infrastructure.FileAccess;
using MusicProcessor.Infrastructure.Persistence;
using MusicProcessor.Infrastructure.Persistence.Repositories;
using MusicProcessor.Infrastructure.SpotDL;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<ISpotDLService, SpotDLService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<ISpotDLMetadataLoader, SpotDLMetadataLoader>();

        var dbPath = Path.Combine(Environment.CurrentDirectory, "music.sqlite");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddTransient<ISongRepository, SongRepository>();
        services.AddTransient<IStyleRepository, StyleRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IArtistRepository, ArtistRepository>();

        return services;
    }
}