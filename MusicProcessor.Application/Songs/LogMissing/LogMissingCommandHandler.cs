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
    ILogger<LogMissingCommandHandler> logger)
    : ICommandHandler<LogMissingCommand>
{
    public async ValueTask<Result> Handle(LogMissingCommand request, CancellationToken ct)
    {
        /* ---------- 1.  read metadata from every physical file ---------- */
        var readSongs = new List<Song>();

        foreach (string path in fileService.GetAllSongPaths())
        {
            try
            {
                readSongs.Add(audioService.ReadMetadata(path));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot read metadata for '{SongPath}'", path);
            }
        }

        /* ---------- 2.  pull the authoritative list from storage ---------- */
        var dbSongs = (await songRepository.GetAllAsync()).ToList();

        /* ---------- 3.  build key sets for O(1) look-ups ---------- */
        // Key comparisons are usually case-insensitive; tweak if you need ordinal.
        var dbKeys = dbSongs.OrderBy(s => s.GetSongKey()).Select(s => s.GetSongKey()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var fileKeys = readSongs.OrderBy(s => s.GetSongKey()).Select(s => s.GetSongKey()).ToHashSet(StringComparer.OrdinalIgnoreCase);

        /* ---------- 4.  physical file exists but no DB row ---------- */
        foreach (Song s in readSongs.Where(s => !dbKeys.Contains(s.GetSongKey())))
        {
            logger.LogWarning("Song NOT in repository  ▶  Key='{SongKey}', ISRC='{ISRC}'", s.GetSongKey(), s.Isrc);
        }

        /* ---------- 5.  (optional)  DB row exists but file is gone ---------- */
        foreach (Song s in dbSongs.Where(s => !fileKeys.Contains(s.GetSongKey())))
        {
            logger.LogWarning("Song missing on disk    ▶  Key='{SongKey}', Title='{ISRC}'", s.GetSongKey(), s.Isrc);
        }

        return Result.Success();
    }
}