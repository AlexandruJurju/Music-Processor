using Microsoft.Extensions.Configuration;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.ExportMetadata;

public class ExportMetadataCommandHandler(
    IMetadataService metadataService,
    ISongRepository songRepository,
    IConfiguration configuration
) : ICommandHandler<ExportMetadataCommand>
{
    public async ValueTask<Result> Handle(ExportMetadataCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Song> songs = await songRepository.GetAllAsync();
        
        await metadataService.ExportMetadataAsync(songs, configuration["Paths:ExportedMetadata"]!);

        return Result.Success();
    }
}
