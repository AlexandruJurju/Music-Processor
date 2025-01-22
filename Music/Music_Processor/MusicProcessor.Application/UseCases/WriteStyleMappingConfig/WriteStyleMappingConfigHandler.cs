using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

internal sealed class WriteStyleMappingConfigHandler(
    IConfigRepository configRepository)
    : IRequestHandler<WriteStyleMappingConfigCommand>
{
    public async Task Handle(WriteStyleMappingConfigCommand request, CancellationToken cancellationToken)
    {
        await configRepository.WriteStyleMappingAsync();
    }
}