using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Domain.Entities.GenreCategories;
using MusicProcessor.Domain.Entities.Genres;
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
        if (!await context.GenreCategories.AnyAsync()) await InitGenres(context);

        // Seed default styles with genre mappings
        if (!await context.Genres.AnyAsync()) await InitStylesWithGenres(context);
    }


    private static async Task InitStylesWithGenres(ApplicationDbContext context)
    {
        var rockGenre = await context.GenreCategories.FirstAsync(g => g.Name == "Rock");
        var synthwaveGenre = await context.GenreCategories.FirstAsync(g => g.Name == "Synthwave");

        var styles = new List<Genre>
        {
            new("Hard Rock")
            {
                GenreCategories = { rockGenre }
            },
            new("Spacewave")
            {
                GenreCategories = { synthwaveGenre }
            },
            new("Canadian Rock")
            {
                GenreCategories = { rockGenre },
                RemoveFromSongs = true
            }
        };

        await context.Genres.AddRangeAsync(styles);
        await context.SaveChangesAsync();
    }

    private static async Task InitGenres(ApplicationDbContext context)
    {
        var genres = new List<GenreCategory>
        {
            new("Rock"),
            new("Synthwave")
        };

        await context.GenreCategories.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }
}