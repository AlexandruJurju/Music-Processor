using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Styles;

namespace MusicProcessor.Infrastructure.Database.Configurations;

public class StyleConfiguration : IEntityTypeConfiguration<Style>
{
    public void Configure(EntityTypeBuilder<Style> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasMany(e => e.Genres)
            .WithMany();

        builder.HasIndex(e => e.Name);
    }
}
