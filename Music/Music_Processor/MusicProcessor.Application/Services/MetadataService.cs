using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Services;

public class MetadataService(IEnumerable<IMetadataHandler> metadataHandlers) : IMetadataService
{
    public void WriteMetadata(Song song)
    {
        var handler = GetMetadataHandler(song.FilePath);
        handler.WriteMetadata(song);
    }

    public Song ReadMetadata(string songFile)
    {
        var handler = GetMetadataHandler(songFile);
        return handler.ReadMetadata(songFile);
    }

    private IMetadataHandler GetMetadataHandler(string songPath)
    {
        return metadataHandlers.First(handler => handler.CanHandle(songPath));
    }
}