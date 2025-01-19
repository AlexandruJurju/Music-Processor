using System.CommandLine;
using Microsoft.Extensions.Logging;
using MusicProcessor.CLI.Commands;

namespace MusicProcessor.CLI;

public class CommandLineApp(
    ILogger<CommandLineApp> logger,
    FixMetadataCommand fixMetadataCommand,
    FirstTimeSyncCommand firstTimeSyncCommand,
    UpdateSyncCommand updateSyncCommand,
    CommitChangesCommand commitChangesCommand
)
{
    public async Task<int> RunAsync(string[] args)
    {
        try
        {
            var rootCommand = new RootCommand("Music Processor CLI application")
            {
                // Add all commands
                fixMetadataCommand.CreateSubCommand(),
                firstTimeSyncCommand.CreateSubCommand(),
                updateSyncCommand.CreateSubCommand(),
                commitChangesCommand.CreateSubCommand()
            };

            return await rootCommand.InvokeAsync(args);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation cancelled by user");
            return 1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing command");
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}