using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Interfaces.Application;

public interface IMetadataService
{
    void WriteMetadata(Song song);
    Song ReadMetadata(string songFile);
}