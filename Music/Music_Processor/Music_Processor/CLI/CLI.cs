using Microsoft.Extensions.Logging;

namespace Music_Processor.CLI;

public class CLI
{
    private readonly ILogger<CLI> _logger;

    public CLI(ILogger<CLI> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            PrintMenu();
            if (!TryGetMenuChoice(out var choice)) continue;

            try
            {
                await HandleChoiceAsync(choice);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nOperation cancelled by user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command");
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
    }

    private async Task HandleChoiceAsync(int choice)
    {
        switch (choice)
        {
            case MenuChoice.FirstTimeSync:
                await FirstTimeSyncAsync();
                break;
            case MenuChoice.UpdateSync:
                await UpdateSyncAsync();
                break;
            case MenuChoice.FixGenres:
                await FixGenresAsync();
                break;
            case MenuChoice.WriteSongList:
                await WriteSongListAsync();
                break;
            case MenuChoice.WriteMetadataFile:
                await WriteMetadataFileAsync();
                break;
            case MenuChoice.ApplyMetadata:
                await ApplyMetadataAsync();
                break;
            case MenuChoice.Exit:
                Exit();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(choice));
        }
    }

    private void Exit()
    {
        Environment.Exit(0);
    }

    private async Task ApplyMetadataAsync()
    {
        throw new NotImplementedException();
    }

    private async Task WriteMetadataFileAsync()
    {
        throw new NotImplementedException();
    }

    private async Task WriteSongListAsync()
    {
        throw new NotImplementedException();
    }

    private async Task FixGenresAsync()
    {
        throw new NotImplementedException();
    }

    private async Task UpdateSyncAsync()
    {
        throw new NotImplementedException();
    }

    private async Task FirstTimeSyncAsync()
    {
        throw new NotImplementedException();
    }

    private void PrintMenu()
    {
        Console.WriteLine("\nMusic Processor Menu:");
        Console.WriteLine($"{MenuChoice.FirstTimeSync}. First Time Sync");
        Console.WriteLine($"{MenuChoice.UpdateSync}. Update Sync");
        Console.WriteLine($"{MenuChoice.FixGenres}. Fix Genres");
        Console.WriteLine($"{MenuChoice.WriteSongList}. Write Song List");
        Console.WriteLine($"{MenuChoice.WriteMetadataFile}. Write Metadata File");
        Console.WriteLine($"{MenuChoice.ApplyMetadata}. Apply Metadata");
        Console.WriteLine($"{MenuChoice.Exit}. Exit");
        Console.Write("\nEnter your choice: ");
    }

    private bool TryGetMenuChoice(out int choice)
    {
        if (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Please enter a valid number.");
            return false;
        }

        if (choice < MenuChoice.FirstTimeSync || choice > MenuChoice.Exit)
        {
            Console.WriteLine($"Please enter a number between {MenuChoice.FirstTimeSync} and {MenuChoice.Exit}.");
            return false;
        }

        return true;
    }
}