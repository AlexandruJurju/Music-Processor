namespace MusicProcessor.Domain.Models.Config;

public record StyleMappingDTO(
    string StyleName,
    bool RemoveFromSongs,
    List<string> GenreNames
);