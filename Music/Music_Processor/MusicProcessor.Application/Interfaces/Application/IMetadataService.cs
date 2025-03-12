using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Interfaces.Application;

public interface IMetadataService
{
    void WriteMetadata(Song song);
    Song ReadMetadata(string songPath);
}