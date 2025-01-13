namespace Music_Processor.CLI.Commands;

public class ExitCommand : IMenuCommand
{
    public string Name => "Exit";

    public Task ExecuteAsync()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}