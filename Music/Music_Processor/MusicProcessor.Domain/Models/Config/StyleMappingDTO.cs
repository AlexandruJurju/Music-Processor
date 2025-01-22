namespace MusicProcessor.Domain.Models.Config;

public record StyleMappingDTO(
    string SongName,
    bool RemoveFromSongs,
    List<string> GenreNames
);