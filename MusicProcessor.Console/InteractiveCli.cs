using CliFx;
using Microsoft.Extensions.Logging;

namespace MusicProcessor.Console;

public class InteractiveCli(
    CliApplication cliApplication,
    ILogger<InteractiveCli> logger
)
{
    public async Task RunInteractiveAsync()
    {
        System.Console.WriteLine("Welcome to Music Processor CLI!");
        System.Console.WriteLine("Type 'help' for available commands or 'exit' to quit");

        while (true)
        {
            System.Console.Write("> ");
            string? input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (input.Equals("exit", StringComparison.Ordinal))
            {
                break;
            }

            string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            await ExecuteCommandAsync(args);
        }
    }

    private async Task ExecuteCommandAsync(string[] args)
    {
        try
        {
            await cliApplication.RunAsync(args);
        }
        catch (OperationCanceledException)
        {
            System.Console.WriteLine("Operation cancelled by user");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing command");
            System.Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
