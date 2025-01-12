using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.Services;

public class SpotDLService : ISpotDLService
{
    private readonly ILogger<SpotDLService> _logger;

    public SpotDLService(ILogger<SpotDLService> logger)
    {
        _logger = logger;
    }

    private async Task RunCommandAsync(string[] command)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "spotdl",
                Arguments = string.Join(" ", command),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Console.WriteLine(args.Data);
                    _logger.LogInformation(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Console.Error.WriteLine(args.Data);
                    _logger.LogError(args.Data);
                }
            };

            process.Start();

            // Start reading output and error streams asynchronously
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"spotdl command failed with exit code {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running spotdl command");
            throw;
        }
    }


    public async Task NewSyncAsync(string playlistUrl, string playlistName, string baseDir)
    {
        var playlistDir = Path.Combine(baseDir, playlistName);
        Directory.CreateDirectory(playlistDir);

        var command = new[]
        {
            playlistUrl,
            "--output", playlistDir,
            "--save-file", Path.Combine(playlistDir, $"{playlistName}.spotdl")
        };

        await RunCommandAsync(command);
    }

    public async Task UpdateSyncAsync(string playlistName, string baseDir)
    {
        var syncFile = Path.Combine(baseDir, playlistName, $"{playlistName}.spotdl");

        if (!File.Exists(syncFile))
        {
            throw new FileNotFoundException($"Sync file not found for playlist {playlistName}");
        }

        var command = new[]
        {
            "sync", syncFile,
            "--output", Path.Combine(baseDir, playlistName)
        };

        await RunCommandAsync(command);
    }
}