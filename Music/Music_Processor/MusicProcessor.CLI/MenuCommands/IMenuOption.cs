namespace MusicProcessor.CLI.MenuCommands;

public interface IMenuOption
{
    string Name { get; }
    Task ExecuteAsync();
}