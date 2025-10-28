using ATL;
using MusicProcessor.Domain;
using MusicProcessor.Infrastructure.Contracts;

namespace MusicProcessor.Infrastructure;

public class AudioService : IAudioService
{
    private const string Styles = "STYLES";
    private const string Artists = "ARTISTS";

    public async Task<Song> ReadMetadataAsync(string songFilePath)
    {
        Track track = new Track(songFilePath);

        var artists = GetArtistsFromArtistString(track.Artist);

        var song = Song.Create(
            track.Title,
            mainArtistName: artists.First(),
            artists: artists,
            styles: GetStyles(track),
            albumName: track.Album,
            discNumber: track.DiscNumber ?? throw new NullReferenceException(),
            discCount: track.DiscTotal ?? throw new NullReferenceException(),
            duration: track.Duration,
            year: track.Year ?? throw new NullReferenceException(),
            trackNumber: track.TrackNumber ?? throw new NullReferenceException(),
            tracksCount: track.TrackTotal ?? throw new NullReferenceException(),
            isrc: track.ISRC ?? throw new NullReferenceException()
        );

        return song;
    }

    private List<string> GetArtistsFromArtistString(string artistString)
    {
        return artistString.Split(';').ToList();
    }

    private List<string> GetStyles(Track track)
    {
        if (track.AdditionalFields.TryGetValue(Styles, out string? value))
        {
            return value.Split(";").ToList();
        }

        return new List<string>();
    }
}