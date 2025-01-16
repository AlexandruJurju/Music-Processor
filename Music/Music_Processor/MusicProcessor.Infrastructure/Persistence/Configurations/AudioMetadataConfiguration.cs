using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Model;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

public class AudioMetadataConfiguration : IEntityTypeConfiguration<AudioMetadata>
{
    public void Configure(EntityTypeBuilder<AudioMetadata> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Album)
            .HasMaxLength(200);

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.Property(e => e.FileType)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.MetadataHash)
            .IsRequired()
            .HasMaxLength(44);

        builder.Property(e => e.Duration)
            .HasConversion(
                v => v.Ticks,
                v => TimeSpan.FromTicks(v));

        // Many-to-many relationships
        builder.HasMany(e => e.Artists)
            .WithMany(e => e.Tracks);

        builder.HasMany(e => e.Genres)
            .WithMany(e => e.Tracks);

        builder.HasMany(e => e.Styles)
            .WithMany(e => e.Tracks);
    }
}