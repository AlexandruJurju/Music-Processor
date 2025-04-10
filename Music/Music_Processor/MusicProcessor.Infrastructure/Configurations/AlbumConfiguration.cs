using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Albums;

namespace MusicProcessor.Persistence.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Type)
            .HasConversion<string>();

        builder.HasOne(a => a.Artist)
            .WithMany()
            .HasForeignKey(a => a.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Songs)
            .WithOne(s => s.Album)
            .HasForeignKey(s => s.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
