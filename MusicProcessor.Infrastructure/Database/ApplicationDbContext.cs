using Microsoft.EntityFrameworkCore;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Genres;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Domain.Styles;

namespace MusicProcessor.Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
) : DbContext(options), IUnitOfWork
{
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Style> Styles { get; set; }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
