namespace Music_Processor.Interfaces;

public interface IMetadataHandler
{
    void UpdateTags(string songPath, string[] genres, string[] styles);
}