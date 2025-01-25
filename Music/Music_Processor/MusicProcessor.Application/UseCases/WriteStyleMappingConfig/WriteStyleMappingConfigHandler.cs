using MediatR;
using MusicProcessor.Application.Abstractions.Infrastructure;

namespace MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

internal sealed class WriteStyleMappingConfigHandler(
    IStyleConfigRepository styleConfigRepository)
    : IRequestHandler<WriteStyleMappingsCommand>
{
    public async Task Handle(WriteStyleMappingsCommand request, CancellationToken cancellationToken)
    {
        await styleConfigRepository.WriteStyleMappingAsync();
    }
}