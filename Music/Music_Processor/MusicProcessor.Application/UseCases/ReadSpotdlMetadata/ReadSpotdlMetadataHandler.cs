using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Genres;
using MusicProcessor.Domain.SongsMetadata;
using MusicProcessor.SpotDL.Interfaces;
using MusicProcessor.SpotDL.Models;

namespace MusicProcessor.Application.UseCases.ReadSpotdlMetadata;

internal sealed class ReadSpotdlMetadataHandler(
    ISpotDLMetadataReader spotDlMetadataReader,
    IFileService fileService,
    IMetadatImportService metadataImportService,
    ILogger<ReadSpotdlMetadataHandler> logger) : IRequestHandler<ReadSpotdlMetadataCommand>
{
    public async Task Handle(ReadSpotdlMetadataCommand request, CancellationToken cancellationToken)
    {
        string? spotdlFile = fileService.GetSpotDLFileInPlaylistFolder(request.PlaylistName);
        Dictionary<string, SpotDLSongMetadata> spotdlMetadata = await spotDlMetadataReader.LoadSpotDLMetadataAsync(spotdlFile!);

        var songsToAdd = new List<SongMetadata>();
        foreach (KeyValuePair<string, SpotDLSongMetadata> spotdlSong in spotdlMetadata)
        {
            songsToAdd.Add(ToSongMetadata(spotdlSong.Value));
        }

        await metadataImportService.ImportSongMetadataAsync(songsToAdd);
    }

    private SongMetadata ToSongMetadata(SpotDLSongMetadata spotdlSongMetadata)
    {
        var artists = spotdlSongMetadata.Artists.Select(x => new Artist(x)).ToList();

        var genres = spotdlSongMetadata.Genres
            .Select(x => new Genre(char.ToUpper(x[0]) + x.Substring(1)))
            .ToList();

        DateOnly.TryParse(spotdlSongMetadata.Date, out DateOnly date);

        var spotifyInfo = new SpotifyInfo(
            spotdlSongMetadata.SongId,
            spotdlSongMetadata.Url,
            spotdlSongMetadata.CoverUrl,
            spotdlSongMetadata.AlbumId,
            spotdlSongMetadata.ArtistId,
            spotdlSongMetadata.Publisher
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
