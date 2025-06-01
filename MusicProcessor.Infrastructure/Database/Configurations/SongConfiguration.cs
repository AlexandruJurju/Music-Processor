using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Infrastructure.Database.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Isrc)
            .HasMaxLength(12);

        builder.HasOne(s => s.MainArtist)
            .WithMany()
            .IsRequired();

        builder.HasOne(s => s.Album)
            .WithMany()
            .IsRequired();

        builder.HasMany(s => s.Artists)
            .WithMany();

        builder.HasMany(s => s.Styles)
            .WithMany();
        
        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.Key);
        builder.HasIndex(x => x.Isrc);
        builder.HasIndex(x => x.Year);
    }
}
