using Microsoft.Extensions.Configuration;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.ExportMetadata;

public class ExportMetadataCommandHandler(
    IMetadataService metadataService,
    ISongRepository songRepository,
    IConfiguration configuration
) : ICommandHandler<ExportMetadataCommand>
{
    public async ValueTask<Result> Handle(ExportMetadataCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Song> songs = await songRepository.GetAllWithReferencesAsync();
        
        await metadataService.ExportMetadataAsync(songs, configuration["Paths:ExportedMetadata"]!);

        return Result.Success();
    }
}
