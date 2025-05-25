using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.ReadMetadataFromFile;

public class ReadMetadataFromFileCommandHandler(
    ISongRepository songRepository,
    ISpotDLMetadataReader spotDlMetadataReader
) : ICommandHandler<ReadMetadataFromFileCommand>
{
#pragma warning disable S1075
    private readonly string _path = "X:\\Storage\\Music\\# SpotDL\\All.spotdl";
#pragma warning restore S1075

    public async ValueTask<Result> Handle(ReadMetadataFromFileCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, Song> songs = songRepository.GetAllWithKey();

        Dictionary<string, Song> spotDlSongs = await spotDlMetadataReader.LoadSpotDLMetadataAsync(_path);

        foreach (KeyValuePair<string, Song> kvp in spotDlSongs)
        {
            if (!songs.ContainsKey(kvp.Key))
            {
                songRepository.Add(kvp.Value);
            }
        }

        return Result.Success();
    }
}
