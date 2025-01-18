namespace MusicProcessor.CLI.MenuCommands;

public class ExitOption : IMenuOption
{
    public string Name => "Exit";

    public Task ExecuteAsync()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}