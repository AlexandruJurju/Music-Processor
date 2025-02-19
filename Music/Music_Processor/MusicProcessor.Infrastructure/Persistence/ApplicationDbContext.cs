﻿using Microsoft.EntityFrameworkCore;
using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Song> Songs { get; set; }
    public DbSet<GenreCategory> GenreCategories { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            switch (entry.State)
            {
                case EntityState.Added:
                    var date = DateTime.UtcNow;
                    entry.Entity.DateCreated = date;
                    entry.Entity.DateModified = date;
                    break;
                case EntityState.Modified:
                    entry.Entity.DateModified = DateTime.UtcNow;
                    break;
            }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}