using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.Test;

public class TestCommandHandler(
    IMetadataService metadataService,
    IFileService fileService
) : ICommandHandler<TestCommand>
{
    public async ValueTask<Result> Handle(TestCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<string> allSongPaths = fileService.GetAllSongPaths();

        foreach (string songPath in allSongPaths)
        {
            Song song = metadataService.ReadMetadata(songPath);
            Console.WriteLine(song.Title);
        }

        await Task.CompletedTask;

        return Result.Success();
    }
}
