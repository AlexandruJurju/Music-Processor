namespace MusicProcessor.Domain.Models.Config;

public record GenreMappingDTO(
    string genreName,
    bool RemoveFromSongs,
    List<string> genreCategoryNames
);