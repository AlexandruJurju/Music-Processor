using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.SongsMetadata;
using Serilog;

namespace MusicProcessor.Application.UseCases.LogMissing;

internal sealed class LogMissingQueryHandler(
    ISongMetadataRepository songMetadataRepository,
    IFileService fileService,
    IMetadataService metadataService,
    ILogger<LogMissingQueryHandler> logger) : IRequestHandler<LogMissingQuery>
{
    public async Task Handle(LogMissingQuery request, CancellationToken cancellationToken)
    {
        Dictionary<string, SongMetadata> playlistMetadata = await songMetadataRepository.GetAllSongsWithKeyAsync();
        IEnumerable<string> songFiles = fileService.GetAllMainMusicFiles();

        foreach (string songFile in songFiles)
        {
            try
            {
                SongMetadata songMetadata = metadataService.ReadMetadata(songFile);
                if (!playlistMetadata.TryGetValue(songMetadata.Key, out SongMetadata? metadata))
                {
                    Log.Error("Unable to find song metadata for \n{Key}\n{Name}", songMetadata.Key, songMetadata.Name);
                    logger.LogError("Unable to find song metadata for \n{Key}\n{Name}", songMetadata.Key, songMetadata.Name);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to read song metadata for {SongFile}", songFile);
            }
        }
    }
}
