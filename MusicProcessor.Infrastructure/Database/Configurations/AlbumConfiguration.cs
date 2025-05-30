using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Albums;

namespace MusicProcessor.Infrastructure.Database.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(a => a.MainArtist)
            .WithMany()
            .IsRequired();

        builder.HasIndex(e => e.Name);
    }
}
