using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly ILogger<PlaylistProcessor> _logger;

    public PlaylistProcessor(ILogger<PlaylistProcessor> logger)
    {
        _logger = logger;
    }

    public void FixPlaylistGenres(string playlistPath)
    {
        
    }
}