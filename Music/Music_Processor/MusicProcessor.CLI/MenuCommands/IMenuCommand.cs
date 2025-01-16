namespace MusicProcessor.CLI.MenuCommands;

public interface IMenuCommand
{
    string Name { get; }
    Task ExecuteAsync();
}