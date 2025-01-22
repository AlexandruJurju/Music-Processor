using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Infrastructure.Persistence;

namespace MusicProcessor.CLI.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedDataAsync(this IHostApplicationBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        // await context.Database.MigrateAsync();

        // Seed default genres
        if (!await context.Genres.AnyAsync()) await InitGenres(context);

        // Seed default styles with genre mappings
        if (!await context.Styles.AnyAsync()) await InitStylesWithGenres(context);
    }


    private static async Task InitStylesWithGenres(ApplicationDbContext context)
    {
        var rockGenre = await context.Genres.FirstAsync(g => g.Name == "Rock");
        var synthwaveGenre = await context.Genres.FirstAsync(g => g.Name == "Synthwave");

        var styles = new List<Style>
        {
            new("Hard Rock")
            {
                Genres = { rockGenre }
            },
            new("Spacewave")
            {
                Genres = { synthwaveGenre }
            },
            new("Canadian Rock")
            {
                Genres = { rockGenre },
                RemoveFromSongs = true
            }
        };

        await context.Styles.AddRangeAsync(styles);
        await context.SaveChangesAsync();
    }

    private static async Task InitGenres(ApplicationDbContext context)
    {
        var genres = new List<Genre>
        {
            new("Rock"),
            new("Synthwave")
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }
}