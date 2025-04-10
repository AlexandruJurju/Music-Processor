namespace MusicProcessor.Domain.SongsMetadata;

public record SpotifyInfo(
    string SpotifySongId,
    string SpotifySongUrl,
    string SpotifyCoverUrl,
    string SpotifyAlbumId,
    string SpotifyArtistId,
    string SpotifyPublisher
);
