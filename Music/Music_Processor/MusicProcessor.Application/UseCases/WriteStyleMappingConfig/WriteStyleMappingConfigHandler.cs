using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;

namespace MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

internal sealed class WriteStyleMappingConfigHandler : IRequestHandler<WriteStyleMappingsCommand>
{
    private readonly IGenreSyncService _genreSyncService;

    public WriteStyleMappingConfigHandler(IGenreSyncService genreSyncService)
    {
        _genreSyncService = genreSyncService;
    }

    public async Task Handle(WriteStyleMappingsCommand request, CancellationToken cancellationToken)
    {
        await _genreSyncService.WriteStyleMappingAsync();
    }
}