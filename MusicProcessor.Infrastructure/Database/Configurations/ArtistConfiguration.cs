using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Artists;

namespace MusicProcessor.Infrastructure.Database.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.Name);
    }
}
