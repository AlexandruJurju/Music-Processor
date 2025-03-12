using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Albums;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.AlbumType)
            .HasConversion<string>();

        builder.HasMany(a => a.Songs)
            .WithOne(s => s.Album)
            .HasForeignKey(s => s.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}