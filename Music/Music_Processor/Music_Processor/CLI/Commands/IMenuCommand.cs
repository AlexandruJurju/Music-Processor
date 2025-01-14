namespace Music_Processor.CLI.Commands;

public interface IMenuCommand
{
    string Name { get; }
    Task ExecuteAsync();
}