using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

internal sealed class WriteStyleMappingConfigHandler(
    IConfigRepository configRepository)
    : IRequestHandler<WriteStyleMappingsCommand>
{
    public async Task Handle(WriteStyleMappingsCommand request, CancellationToken cancellationToken)
    {
        await configRepository.WriteStyleMappingAsync();
    }
}