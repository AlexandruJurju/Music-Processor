using System.Collections.Immutable;

namespace Music_Processor.Constants;

public static class Constants
{
    public const string PlaylistFolder = "Playlists";

    public static readonly string[] AudioFileFormats = { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".alac", ".aiff", ".opus" };
    
    public static readonly string[] ProcessableAudioFileFormats = { ".mp3", ".flac" };
    
    public static readonly Dictionary<string, List<string>> DEFAULT_STYLE_MAPPINGS =
        new()
        {
            ["Rock"] = new() { "rock", "alternative rock" },
            ["Synthwave"] = new() { "synthwave", "darksynth" },
            ["Pop"] = new() { "pop", "dance pop" }
        };

    public static readonly List<string> DEFAULT_STYLES_TO_REMOVE = new()
        { "britpop", "madchester" };

    public const string StyleMappingFile = "genre_style_mappings.json";
    public const string StylesToRemoveFile = "styles_to_remove.json";
}