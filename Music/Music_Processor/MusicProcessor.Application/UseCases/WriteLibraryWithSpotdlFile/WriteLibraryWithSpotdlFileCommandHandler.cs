using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Factories;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

public class WriteLibraryWithSpotdlFileCommandHandler(
    ISpotDLMetadataLoader spotDlMetadataLoader,
    MetadataHandlerFactory metadataHandlerFactory,
    IFileService fileService,
    ILogger<WriteLibraryWithSpotdlFileCommandHandler> logger,
    ISongProcessor songProcessor)
    : IRequestHandler<WriteLibraryWithSpotdlFileCommand>
{
    public async Task Handle(WriteLibraryWithSpotdlFileCommand request, CancellationToken cancellationToken)
    {
        var songsToAdd = new List<Song>();

        logger.LogInformation("{Message}", $"Loading SpotDL metadata from {request.PlaylistPath}");
        Dictionary<string, Song> spotdlMetadata = await spotDlMetadataLoader.LoadSpotDLMetadataAsync(request.PlaylistPath);
        logger.LogInformation("{Message}", $"Found {spotdlMetadata.Count} songs in SpotDL metadata");

        var playlistSongs = fileService.GetAllAudioFilesInFolder(request.PlaylistPath);
        logger.LogInformation("{Message}", $"Found {playlistSongs.Count()} audio files in folder");

        foreach (var songFile in playlistSongs)
        {
            logger.LogInformation("{Message}", $"Processing file: {Path.GetFileName(songFile)}");

            var handler = metadataHandlerFactory.GetHandler(songFile);
            var songMetadata = handler.ExtractMetadata(songFile);
            var spotdlSongKey = spotDlMetadataLoader.CreateLookupKey(songMetadata.Artists, songMetadata.Title);

            logger.LogInformation("{Message}", $"Looking up song with key: {spotdlSongKey}");

            if (!spotdlMetadata.TryGetValue(spotdlSongKey, out var spotdlSongMetadata))
            {
                logger.LogWarning("{Message}", $"Cannot find song: {songMetadata.Title} in spotdl file, key: {spotdlSongKey}");
                continue;
            }

            spotdlSongMetadata.FilePath = songFile;
            songsToAdd.Add(spotdlSongMetadata);

            logger.LogInformation("{Message}", $"Added song: {spotdlSongMetadata.Title}");
            logger.LogInformation("{Message}", $"Artists: {string.Join(", ", spotdlSongMetadata.Artists.Select(a => a.Name))}");
            logger.LogInformation("{Message}", $"Genres: {string.Join(", ", spotdlSongMetadata.Genres.Select(g => g.Name))}");
            logger.LogInformation("{Message}", $"Styles: {string.Join(", ", spotdlSongMetadata.Styles.Select(s => s.Name))}");
        }

        if (songsToAdd.Any())
        {
            logger.LogInformation("{Message}", $"Saving {songsToAdd.Count} songs to database");
            await songProcessor.AddRawSongToDbAsync(songsToAdd);
            logger.LogInformation("{Message}", "Successfully saved songs to database");
        }
        else
        {
            logger.LogWarning("{Message}", "No songs were found to add to the database");
        }
    }
}