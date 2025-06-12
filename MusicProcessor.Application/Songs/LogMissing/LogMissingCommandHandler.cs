
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.LogMissing;

public class LogMissingCommandHandler(
    IFileService fileService,
    IAudioService audioService,
    ISongRepository songRepository,
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

        IEnumerable<Song> metadata = await songRepository.GetAllAsync();

        foreach (Song song in readSongs)
        {
            IEnumerable<Song> enumerable = metadata as Song[] ?? metadata.ToArray();
            if (enumerable.All(e => e.Key != song.Key))
            {
                logger.LogWarning("Missing Key {SongKey}", song.Key);
            }
        }

        return Result.Success();
    }
}
