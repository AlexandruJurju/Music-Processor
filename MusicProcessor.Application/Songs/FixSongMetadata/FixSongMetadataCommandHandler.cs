using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.FixSongMetadata;

public class FixSongMetadataCommandHandler(
) : ICommandHandler<FixSongMetadataCommand>
{
    public ValueTask<Result> Handle(FixSongMetadataCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
