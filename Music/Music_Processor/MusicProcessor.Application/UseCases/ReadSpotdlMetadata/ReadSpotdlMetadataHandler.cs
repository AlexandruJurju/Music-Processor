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

public class ReadSpotdlMetadataHandler
{
    private readonly IFileService _fileService;
    private readonly IMetadatImportService _metadataImportService;
    private readonly ISpotDLMetadataReader _spotDlMetadataReader;    
    private readonly ILogger<ReadSpotdlMetadataHandler> _logger;
    
    public ReadSpotdlMetadataHandler(
        ISpotDLMetadataReader spotDlMetadataReader,
        IFileService fileService,
        ILogger<ReadSpotdlMetadataHandler> logger,
        IMetadataService metadataService,
        IMetadatImportService metadataImportService)
    {
        _spotDlMetadataReader = spotDlMetadataReader;
        _fileService = fileService;
        _logger = logger;
        _metadataImportService = metadataImportService;
    }

    public async Task Handle(ReadSpotdlMetadataCommand command, CancellationToken cancellationToken)
    {
        var spotdlFile = _fileService.GetSpotDLFileInPlaylistFolder(command.PlaylistName);
        var spotdlMetadata = await _spotDlMetadataReader.LoadSpotDLMetadataAsync(spotdlFile!);

        var songsToAdd = new List<SongMetadata>();
        foreach (var spotdlSong in spotdlMetadata)
        {
            songsToAdd.Add(ToSongMetadata(spotdlSong.Value));
        }

        await _metadataImportService.ImportSongMetadataAsync(songsToAdd);
    }

    private SongMetadata ToSongMetadata(SpotDLSongMetadata spotdlSongMetadata)
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