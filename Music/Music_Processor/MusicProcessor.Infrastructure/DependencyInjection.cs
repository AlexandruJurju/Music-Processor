using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Infrastructure.FileService;
using MusicProcessor.Infrastructure.Repositories;
using MusicProcessor.Persistence.Common;
using MusicProcessor.Persistence.Repositories;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDb(services, configuration);
        AddRepositories(services);

        services.Configure<PathsOptions>(configuration.GetSection("PathsSettings"));

        AddExternalFileServices(services);

        return services;
    }

    private static void AddExternalFileServices(IServiceCollection services)
    {
        services.AddTransient<IFileService, PhysicalFileService>();
        services.AddTransient<IExportService, ExportService.ExportService>();
    }

    private static void AddDb(IServiceCollection services, IConfiguration configuration)
    {
        string? dbPath = configuration.GetValue<string>("PathsSettings:SQLLiteConnection");

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
