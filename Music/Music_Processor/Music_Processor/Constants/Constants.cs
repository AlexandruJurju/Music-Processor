namespace Music_Processor.Constants;

public static class Constants
{
    public const string PlaylistFolder = "Playlists";

    public const string StyleMappingFile = "genre_style_mappings.json";
    public const string StylesToRemoveFile = "styles_to_remove.json";

    public static readonly string[] AudioFileFormats = { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".alac", ".aiff", ".opus" };

    public static readonly string[] ProcessableAudioFileFormats = { ".mp3", ".flac" };

    public static readonly Dictionary<string, List<string>> DEFAULT_STYLE_MAPPINGS =
        new()
        {
            ["Rock"] = new List<string> { "rock", "alternative rock" },
            ["Synthwave"] = new List<string> { "synthwave", "darksynth" },
            ["Pop"] = new List<string> { "pop", "dance pop" }
        };

    public static readonly List<string> DEFAULT_STYLES_TO_REMOVE = new()
        { "britpop", "madchester" };
}