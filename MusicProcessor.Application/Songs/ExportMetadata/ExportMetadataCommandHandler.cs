using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.ExportMetadata;

public class ExportMetadataCommandHandler(
    IMetadataService metadataService
) : ICommandHandler<ExportMetadataCommand>
{
    public async ValueTask<Result> Handle(ExportMetadataCommand request, CancellationToken cancellationToken)
    {
        await metadataService.ExportMetadataAsync();

        return Result.Success();
    }
}
