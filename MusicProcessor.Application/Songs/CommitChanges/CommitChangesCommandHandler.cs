using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.CommitChanges;

public class CommitChangesCommandHandler : ICommandHandler<CommitChangesCommand>
{
    public ValueTask<Result> Handle(CommitChangesCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
