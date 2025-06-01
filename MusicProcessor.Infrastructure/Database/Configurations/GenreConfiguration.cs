using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Genres;

namespace MusicProcessor.Infrastructure.Database.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.Name);
    }
}
