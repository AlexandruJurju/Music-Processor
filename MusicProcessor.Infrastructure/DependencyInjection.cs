using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Infrastructure.Database;
using MusicProcessor.Infrastructure.Database.Repositories;
using MusicProcessor.Infrastructure.MetadataReaders;

namespace MusicProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabase(services, configuration);

        AddRepositories(services);

        AddHealthChecks(services);

        services.AddScoped<ISpotDLMetadataReader, SpotDLMetadataReader>();
        services.AddScoped<IFileService, FileService.FileService>();

        return services;
    }

    private static void AddHealthChecks(IServiceCollection services)
    {
        services
            .AddHealthChecks();
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")!;

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={connectionString}");
            options.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ISongRepository, SongRepository>();
    }
}
