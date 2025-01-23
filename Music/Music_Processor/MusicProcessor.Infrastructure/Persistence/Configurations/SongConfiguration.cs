using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(e => e.Title);

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
                v => v.TotalSeconds,
                v => TimeSpan.FromSeconds(v));

        // Many-to-many relationship with Artist
        builder.HasMany(e => e.Artists)
            .WithMany(e => e.Songs)
            .UsingEntity("song_artists");

        // Many-to-many relationship with Style
        builder.HasMany(e => e.Styles)
            .WithMany(e => e.Songs)
            .UsingEntity("song_styles");
    }
}