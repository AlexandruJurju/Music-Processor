namespace Music_Processor.CLI.Commands;

public interface ICommand
{
    string Name { get; }
    int MenuNumber { get; }
    Task ExecuteAsync();
}