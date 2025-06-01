using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.FixSongMetadata;

public class FixSongMetadataCommandHandler(
) : ICommandHandler<FixSongMetadataCommand>
{
    public ValueTask<Result> Handle(FixSongMetadataCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
