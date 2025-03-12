using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Infrastructure.Persistence.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        ConfigureProperties(builder);
        ConfigureRelationships(builder);
        ConfigureSpotifyInfo(builder);
    }

    private void ConfigureProperties(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(s => s.Name);

        // ISRC is typically 12 characters long
        builder.Property(s => s.ISRC)
            .HasMaxLength(12);

        builder.Property(s => s.Year);

        builder.Property(s => s.Date);

        builder.Property(s => s.TrackNumber);

        builder.Property(s => s.TracksCount);

        builder.Property(s => s.DiscNumber);

        builder.Property(s => s.DiscCount);

        builder.Property(s => s.FileType)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Duration);

        builder.Property(s => s.FilePath)
            .IsRequired()
            .HasMaxLength(500);
    }

    private void ConfigureRelationships(EntityTypeBuilder<Song> builder)
    {
        // MainArtist relationship (one-to-many)
        builder.HasOne(s => s.MainArtist)
            .WithMany()
            .HasForeignKey(s => s.MainArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        // Album relationship (many-to-one)
        builder.HasOne(s => s.Album)
            .WithMany(a => a.Songs)
            .HasForeignKey(s => s.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many relationship with Artist
        builder.HasMany(s => s.Artists)
            .WithMany(a => a.Songs)
            .UsingEntity("song_artists");

        // Many-to-many relationship with Genre
        builder.HasMany(s => s.Genres)
            .WithMany(g => g.Songs)
            .UsingEntity("song_genres");
    }

    private void ConfigureSpotifyInfo(EntityTypeBuilder<Song> builder)
    {
        // Configure SpotifyInfo as an owned entity
        builder.OwnsOne(s => s.SpotifyInfo, spotifyInfo =>
        {
            spotifyInfo.Property(si => si.SpotifySongId).HasMaxLength(50); // Spotify song ID
            spotifyInfo.Property(si => si.SpotifySongUrl).HasMaxLength(500); // Spotify song URL
            spotifyInfo.Property(si => si.SpotifyCoverUrl).HasMaxLength(500); // Spotify cover URL
            spotifyInfo.Property(si => si.SpotifyAlbumId).HasMaxLength(50); // Spotify album ID
            spotifyInfo.Property(si => si.SpotifyArtistId).HasMaxLength(50); // Spotify artist ID
        });
    }
}