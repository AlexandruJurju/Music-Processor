using Microsoft.Extensions.Configuration;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.ExportMissingSongsMetadata;

public sealed class ExportMissingSongsMetadataCommandHandler(
    IAudioService audioService,
    ISongRepository songRepository,
    IFileService fileService,
    IMetadataService metadataService,
    IConfiguration configuration)
    : ICommandHandler<ExportMissingSongsMetadataCommand>
{
    public async ValueTask<Result> Handle(
        ExportMissingSongsMetadataCommand request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Song> allSongs = await songRepository.GetAllAsync();
        var allSongKeys = new HashSet<string>(allSongs.Select(s => s.Key));

        IEnumerable<string> songPaths = fileService.GetAllSongPaths();
        var physicalSongsMetadata = new List<Song>();

        foreach (string songPath in songPaths)
        {
            Song physicalSongMetadata = audioService.ReadMetadata(songPath);

            if (!allSongKeys.Contains(physicalSongMetadata.Key))
            {
                physicalSongsMetadata.Add(physicalSongMetadata);
            }
        }

        string exportPath = configuration["Paths:MissingPhysicalMetadata"]!;

        await metadataService.ExportMetadataAsync(physicalSongsMetadata, exportPath);

        return Result.Success();
    }
}
