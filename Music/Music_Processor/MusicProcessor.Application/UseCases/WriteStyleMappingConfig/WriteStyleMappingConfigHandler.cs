using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;

namespace MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

internal sealed class WriteStyleMappingConfigHandler(
    IStyleSyncService styleSyncService)
    : IRequestHandler<WriteStyleMappingsCommand>
{
    public async Task Handle(WriteStyleMappingsCommand request, CancellationToken cancellationToken)
    {
        await styleSyncService.WriteStyleMappingAsync();
    }
}