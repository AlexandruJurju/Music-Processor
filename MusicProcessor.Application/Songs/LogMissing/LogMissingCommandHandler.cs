using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.LogMissing;

public class LogMissingCommandHandler(
    IFileService fileService,
    IAudioService audioService,
    IMetadataService metadataService,
    ILogger<LogMissingCommandHandler> logger
) : ICommandHandler<LogMissingCommand>
{
    public async ValueTask<Result> Handle(LogMissingCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<string> allSongs = fileService.GetAllSongPaths();

        List<Song> readSongs = new();

        foreach (string songPath in allSongs)
        {
            try
            {
                readSongs.Add(audioService.ReadMetadata(songPath));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading metadata for {SongPath}", songPath);
            }
        }

        List<SpotDLSongMetadata> metadata = await metadataService.LoadSpotDLMetadataAsync();

        foreach (Song song in readSongs)
        {
            if (metadata.All(e => e.Key != song.Key))
            {
                logger.LogWarning("Missing Key {SongKey}", song.Key);
            }
        }

        return Result.Success();
    }
}
