using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.GenreCategories;

namespace MusicProcessor.Persistence.GenreCategories;

public class GenreCategoryConfiguration : IEntityTypeConfiguration<GenreCategory>
{
    public void Configure(EntityTypeBuilder<GenreCategory> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        // Many-to-many relationship with GenreCategory
        builder.HasMany(e => e.Genres)
            .WithMany(e => e.GenreCategories)
            .UsingEntity("genre_genre_category");
    }
}