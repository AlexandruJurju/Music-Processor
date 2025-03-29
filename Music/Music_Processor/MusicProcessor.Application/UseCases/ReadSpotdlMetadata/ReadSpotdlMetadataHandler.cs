using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.SongsMetadata;
using MusicProcessor.SpotDL.Interfaces;
using MusicProcessor.SpotDL.Models;

namespace MusicProcessor.Application.UseCases.ReadSpotdlMetadata;

public class ReadSpotdlMetadataHandler : IRequestHandler<ReadSpotdlMetadataCommand>
{
    private readonly IFileService _fileService;
    private readonly ILogger<ReadSpotdlMetadataHandler> _logger;
    private readonly ISongMetadataRepository _songMetadataRepository;
    private readonly ISongProcessor _songProcessor;
    private readonly ISpotDLMetadataReader _spotDlMetadataReader;

    public ReadSpotdlMetadataHandler(
        ISpotDLMetadataReader spotDlMetadataReader,
        IFileService fileService,
        ISongMetadataRepository songMetadataRepository,
        ILogger<ReadSpotdlMetadataHandler> logger, IMetadataService metadataService, ISongProcessor songProcessor)
    {
        _spotDlMetadataReader = spotDlMetadataReader;
        _fileService = fileService;
        _songMetadataRepository = songMetadataRepository;
        _logger = logger;
        _songProcessor = songProcessor;
    }

    public async Task Handle(ReadSpotdlMetadataCommand request, CancellationToken cancellationToken)
    {
        var spotdlFile = _fileService.GetSpotDLFileInPlaylistFolder(request.PlaylistName);
        var spotdlMetadata = await _spotDlMetadataReader.LoadSpotDLMetadataAsync(spotdlFile!);

        var songsToAdd = new List<SongMetadata>();
        foreach (var spotdlSong in spotdlMetadata)
        {
            songsToAdd.Add(ToSongMetadata(spotdlSong.Value));
        }

        await _songProcessor.ImportSongMetadataAsync(songsToAdd);
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