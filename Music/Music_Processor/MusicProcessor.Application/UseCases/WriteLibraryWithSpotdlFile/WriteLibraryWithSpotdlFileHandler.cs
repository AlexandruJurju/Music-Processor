using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Constants;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

public class WriteLibraryWithSpotdlFileHandler : IRequestHandler<WriteLibraryWithSpotdlFileCommand>
{
    private readonly IFileService _fileService;
    private readonly ILogger<WriteLibraryWithSpotdlFileHandler> _logger;
    private readonly IMetadataService _metadataService;
    private readonly ISongProcessor _songProcessor;
    private readonly ISpotDLMetadataLoader _spotDlMetadataLoader;

    public WriteLibraryWithSpotdlFileHandler(ISpotDLMetadataLoader spotDlMetadataLoader,
        IMetadataService metadataService,
        IFileService fileService,
        ISongProcessor songProcessor,
        ILogger<WriteLibraryWithSpotdlFileHandler> logger)
    {
        _spotDlMetadataLoader = spotDlMetadataLoader;
        _metadataService = metadataService;
        _fileService = fileService;
        _songProcessor = songProcessor;
        _logger = logger;
    }

    public async Task Handle(WriteLibraryWithSpotdlFileCommand request, CancellationToken cancellationToken)
    {
        var songsToAdd = new List<Song>();

        Dictionary<string, Song> spotdlMetadata = await _spotDlMetadataLoader.LoadSpotDLMetadataAsync(request.PlaylistPath);
        _logger.LogDebug($"Found {spotdlMetadata.Count} songs in SpotDL metadata");

        var playlistSongs = _fileService.GetAllAudioFilesInFolder(request.PlaylistPath);
        _logger.LogDebug($"Found {playlistSongs.Length} audio files in folder");

        foreach (var songFile in playlistSongs)
        {
            _logger.LogDebug($"Processing file: {Path.GetFileName(songFile)}");

            var songMetadata = _metadataService.ReadMetadata(songFile);

            var spotdlSongKey = _spotDlMetadataLoader.CreateLookupKey(songMetadata.MainArtist, songMetadata.Name);

            _logger.LogDebug("Looking up song {SongMetadataName} with key: {SpotDlSongKey}", songMetadata.Name, spotdlSongKey);

            if (!spotdlMetadata.TryGetValue(spotdlSongKey, out var spotdlSongMetadata))
            {
                _logger.LogWarning($"Cannot find song:\n{spotdlSongKey}\n{songMetadata.Name}");
                continue;
            }

            spotdlSongMetadata.FilePath = songFile;
            spotdlSongMetadata.FileType = FileTypes.GetFileType(Path.GetExtension(songFile)) ?? "";
            songsToAdd.Add(spotdlSongMetadata);

            _logger.LogDebug($"Added song: {spotdlSongMetadata.Name}");
            _logger.LogDebug($"Artists: {string.Join(", ", spotdlSongMetadata.Artists.Select(a => a.Name))}");
            _logger.LogDebug($"Genres: {string.Join(", ", spotdlSongMetadata.Genres.Select(genre => genre.Name))}");
            _logger.LogDebug($"GenreCategories: {string.Join(", ", spotdlSongMetadata.Genres.SelectMany(genreCategory => genreCategory.GenreCategories.Select(g => g.Name)))}");
        }

        _logger.LogDebug($"Saving {songsToAdd.Count} songs to database");
        await _songProcessor.AddRawSongsToDbAsync(songsToAdd);
        _logger.LogInformation("Successfully saved songs to database");
    }
}