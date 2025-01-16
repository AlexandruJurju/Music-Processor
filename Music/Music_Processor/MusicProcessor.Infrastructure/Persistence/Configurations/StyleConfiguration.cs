using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

public class StyleConfiguration : IEntityTypeConfiguration<Style>
{
    public void Configure(EntityTypeBuilder<Style> builder)
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

        // The many-to-many relationship is configured in GenreConfiguration
    }
}