using Microsoft.Extensions.Logging;
using Music_Processor.CLI.Commands;
using Music_Processor.Factories;

namespace Music_Processor.CLI;

public class CLI
{
    private readonly ILogger<CLI> _logger;
    private readonly MenuCommandFactory _menuCommandFactory;

    public CLI(ILogger<CLI> logger, MenuCommandFactory menuCommandFactory)
    {
        _logger = logger;
        _menuCommandFactory = menuCommandFactory;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            PrintMenu();
            if (!TryGetMenuChoice(out var choice)) continue;

            try
            {
                var command = _menuCommandFactory.GetCommand(choice);
                await command.ExecuteAsync();
                Console.WriteLine();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation cancelled by user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void PrintMenu()
    {
        Console.WriteLine("Music Processor Menu:");
        foreach (var command in _menuCommandFactory.GetAllCommands().OrderBy(c => c.MenuNumber))
        {
            Console.WriteLine($"{command.MenuNumber}. {command.Name}");
        }

        Console.Write("Enter your choice: ");
    }

    private bool TryGetMenuChoice(out int choice)
    {
        if (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Please enter a valid number.");
            return false;
        }

        if (choice < MenuChoices.FirstTimeSync || choice > MenuChoices.Exit)
        {
            Console.WriteLine($"Please enter a number between {MenuChoices.FirstTimeSync} and {MenuChoices.Exit}.");
            return false;
        }

        return true;
    }
}