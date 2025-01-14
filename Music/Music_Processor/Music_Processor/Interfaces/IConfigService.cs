namespace Music_Processor.Interfaces;

public interface IConfigService
{
    Dictionary<string, List<string>> LoadStyleMappingFile();
    List<string> LoadStylesToRemove();
}