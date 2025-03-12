namespace MusicProcessor.Domain.Entities.Songs;

public record SpotifyInfo(
    string SpotifySongId,
    string SpotifySongUrl,
    string SpotifyCoverUrl,
    string SpotifyAlbumId,
    string SpotifyArtistId
);