using Microsoft.Extensions.Configuration;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.PhysicalSongs.ExportMetadata;

public class ExportMetadataCommandHandler (
    IAudioService audioService, 
    IFileService fileService,
    IMetadataService metadataService,
    IConfiguration configuration
    ): ICommandHandler<ExportMetadataCommand>
{
    public async ValueTask<Result> Handle(ExportMetadataCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<string> songPaths =  fileService.GetAllSongPaths();

        List<Song> physicalSongsMetadata = new();
        
        foreach (string songPath in songPaths)
        {
            physicalSongsMetadata.Add(audioService.ReadMetadata(songPath));
        }
        
        await metadataService.ExportMetadataAsync(physicalSongsMetadata, configuration["Paths:ExportedPhysicalMetadata"]!);
        
        return Result.Success();
    }
}
