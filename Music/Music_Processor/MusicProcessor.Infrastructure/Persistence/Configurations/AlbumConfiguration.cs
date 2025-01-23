using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasMany(a => a.Songs)
            .WithOne(s => s.Album)
            .HasForeignKey(s => s.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}