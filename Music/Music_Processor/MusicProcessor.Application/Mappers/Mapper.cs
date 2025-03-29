using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.SongsMetadata;
using MusicProcessor.SpotDL.Models;
using TagLib;

namespace MusicProcessor.Application.Mappers;

public static class Mapper
{
    public static SongMetadata ToSongMetadata(this SpotDLSongMetadata spotdlSongMetadata)
    {
        var artists = spotdlSongMetadata.Artists.Select(x => new Artist(x)).ToList();

        var genres = spotdlSongMetadata.Genres
            .Select(x => new Genre(char.ToUpper(x[0]) + x.Substring(1)))
            .ToList();

        DateOnly.TryParse(spotdlSongMetadata.Date, out var date);

        var spotifyInfo = new SpotifyInfo(
            spotdlSongMetadata.SongId,
            spotdlSongMetadata.Url,
            spotdlSongMetadata.CoverUrl,
            spotdlSongMetadata.AlbumId,
            spotdlSongMetadata.ArtistId
        );

        return new SongMetadata(
            spotdlSongMetadata.Name,
            spotdlSongMetadata.ISRC,
            artists,
            new Artist(spotdlSongMetadata.Artist),
            genres,
            spotdlSongMetadata.DiscNumber,
            spotdlSongMetadata.DiscCount,
            new Album(spotdlSongMetadata.AlbumName, new Artist(spotdlSongMetadata.AlbumArtist)),
            spotdlSongMetadata.Duration,
            int.Parse(spotdlSongMetadata.Year),
            spotdlSongMetadata.TrackNumber,
            spotdlSongMetadata.TracksCount,
            spotifyInfo
        );
    }
}