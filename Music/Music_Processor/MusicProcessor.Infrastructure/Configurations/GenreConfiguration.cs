using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Genres;

namespace MusicProcessor.Persistence.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.RemoveFromSongs)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        // Many-to-many relationship with GenreCategory
        builder.HasMany(e => e.GenreCategories)
            .WithMany(e => e.Genres)
            .UsingEntity("genre_genre_category");

        // Many-to-many relationship with SongMetadata
        builder.HasMany(e => e.Songs)
            .WithMany(e => e.Genres)
            .UsingEntity("song_genres");
    }
}
