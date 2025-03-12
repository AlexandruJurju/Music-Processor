using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Entities.GenreCategories;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

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

        // Many-to-many relationship with GenreConfiguration
        builder.HasMany(e => e.Genres)
            .WithMany(e => e.GenreCategories)
            .UsingEntity("genre_genre_category");
    }
}