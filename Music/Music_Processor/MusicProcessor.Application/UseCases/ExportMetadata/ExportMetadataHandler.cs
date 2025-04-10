using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.UseCases.ExportMetadata;

internal sealed class ExportMetadataHandler(
    IExportService exportService,
    ISongMetadataRepository songMetadataRepository) : IRequestHandler<ExportMetadataCommand>
{
    public async Task Handle(ExportMetadataCommand request, CancellationToken cancellationToken)
    {
        List<SongMetadata> songs = await songMetadataRepository.GetAll()
            .ToListAsync(cancellationToken);

        await exportService.ExportMetadata(songs);
    }
}
