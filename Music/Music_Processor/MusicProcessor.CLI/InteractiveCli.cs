using CliFx;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace MusicProcessor.CLI;

public class InteractiveCli
{
    private readonly CliApplication _cliApplication;
    private readonly ILogger<InteractiveCli> _logger;

    public InteractiveCli(ILogger<InteractiveCli> logger, CliApplication cliApplication)
    {
        _logger = logger;
        _cliApplication = cliApplication;
    }

    public async Task RunInteractiveAsync()
    {
        Console.WriteLine("Welcome to Music Processor CLI!");
        Console.WriteLine("Type 'help' for available commands or 'exit' to quit");

        
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
        
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.ToLower() == "exit") break;
        
            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            await ExecuteCommandAsync(args);
        }
    }

    private async Task ExecuteCommandAsync(string[] args)
    {
        try
        {
            await _cliApplication.RunAsync(args);
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