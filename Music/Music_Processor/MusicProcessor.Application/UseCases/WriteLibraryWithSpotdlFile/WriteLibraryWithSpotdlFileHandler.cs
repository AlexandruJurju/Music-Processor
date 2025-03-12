using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Constants;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

public class WriteLibraryWithSpotdlFileHandler : IRequestHandler<WriteLibraryWithSpotdlFileCommand>
{
    private readonly ISpotDLMetadataLoader _spotDlMetadataLoader;
    private readonly IMetadataService _metadataService;
    private readonly IFileService _fileService;
    private readonly ISongProcessor _songProcessor;
    private readonly ILogger<WriteLibraryWithSpotdlFileHandler> _logger;

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

        _logger.LogInformation("{Message}", $"Loading SpotDL metadata from {request.PlaylistPath}");
        Dictionary<string, Song> spotdlMetadata = await _spotDlMetadataLoader.LoadSpotDLMetadataAsync(request.PlaylistPath);
        _logger.LogInformation("{Message}", $"Found {spotdlMetadata.Count} songs in SpotDL metadata");

        var playlistSongs = _fileService.GetAllAudioFilesInFolder(request.PlaylistPath);
        _logger.LogInformation("{Message}", $"Found {playlistSongs.Count()} audio files in folder");

        foreach (var songFile in playlistSongs)
        {
            _logger.LogInformation("{Message}", $"Processing file: {Path.GetFileName(songFile)}");

            var songMetadata = _metadataService.ReadMetadata(songFile);

            var spotdlSongKey = _spotDlMetadataLoader.CreateLookupKey(songMetadata.MainArtist, songMetadata.Title);

            _logger.LogInformation("{Message}", $"Looking up song {songMetadata.Title} with key: {spotdlSongKey}");

            if (!spotdlMetadata.TryGetValue(spotdlSongKey, out var spotdlSongMetadata))
            {
                _logger.LogError("{Message}", $"Cannot find song:\n{spotdlSongKey}\n{songMetadata.Title}");
                continue;
            }

            spotdlSongMetadata.FilePath = songFile;
            spotdlSongMetadata.FileType = FileTypes.GetFileType(Path.GetExtension(songFile)) ?? "";
            // songsToAdd.Add(spotdlSongMetadata);

            _logger.LogInformation("{Message}", $"Added song: {spotdlSongMetadata.Title}");
            _logger.LogInformation("{Message}", $"Artists: {string.Join(", ", spotdlSongMetadata.Artists.Select(a => a.Name))}");
            _logger.LogInformation("{Message}", $"Genres: {string.Join(", ", spotdlSongMetadata.Genres.Select(genre => genre.Name))}");
            _logger.LogInformation("{Message}", $"GenreCategories: {string.Join(", ", spotdlSongMetadata.Genres.SelectMany(genreCategory => genreCategory.GenreCategories.Select(g => g.Name)))}");
        }

        if (songsToAdd.Count != 0)
        {
            _logger.LogInformation("{Message}", $"Saving {songsToAdd.Count} songs to database");
            await _songProcessor.AddRawSongsToDbAsync(songsToAdd);
            _logger.LogInformation("{Message}", "Successfully saved songs to database");
        }
        else
        {
            _logger.LogWarning("{Message}", "No songs were found to add to the database");
        }
    }
}