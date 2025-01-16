using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLService : ISpotDLService
{
    private readonly List<string> _downloadErrors = new();
    private readonly ILogger<SpotDLService> _logger;
    private readonly List<string> _lookupErrors = new();

    public SpotDLService(ILogger<SpotDLService> logger)
    {
        _logger = logger;
    }


    public async Task NewSyncAsync(string playlistUrl, string playlistName, string baseDir)
    {
        var playlistDir = Path.Combine(baseDir, playlistName);
        Directory.CreateDirectory(playlistDir);

        var command = new[]
        {
            "sync",
            playlistUrl,
            "--output",
            playlistDir,
            "--save-file",
            Path.Combine(playlistDir, $"{playlistName}.spotdl")
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
            "sync",
            syncFile,
            "--output",
            Path.Combine(baseDir, playlistName)
        };

        await RunCommandAsync(command);
    }

    private void ProcessStandardOutput(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        if (IsErrorOutput(data))
        {
            WriteErrorOutput(data);
            ProcessErrorData(data);
        }
        else
        {
            WriteSuccessOutput(data);
        }
    }

    private bool IsErrorOutput(string data)
    {
        return data.Contains("LookupError: No results found for song:") ||
               data.Contains("AudioProviderError: YT-DLP download error") ||
               data.Trim().StartsWith("https://music.youtube.com/watch?v=");
    }

    private void WriteSuccessOutput(string data)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(data);
        Console.ResetColor();
    }

    private void WriteErrorOutput(string data)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(data);
        Console.ResetColor();
    }

    private void ProcessErrorData(string data)
    {
        if (data.Contains("LookupError: No results found for song:"))
        {
            var songName = data.Replace("LookupError: No results found for song:", "").Trim();
            _lookupErrors.Add(songName);
        }
        // else if (data.Contains("AudioProviderError: YT-DLP download error"))
        // {
        //     var nextLine = data.Split('\n').LastOrDefault();
        //     if (!string.IsNullOrEmpty(nextLine))
        //     {
        //         _downloadErrors.Add(nextLine.Trim());
        //     }
        // }
        else if (data.Trim().StartsWith("https://music.youtube.com/watch?v="))
        {
            _downloadErrors.Add(data.Trim());
        }
    }

    private void PrintMissingSongs()
    {
        if (_lookupErrors.Count == 0 && _downloadErrors.Count == 0) return;

        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine("\n=== Missing Songs Summary ===");

        if (_lookupErrors.Count > 0)
        {
            Console.WriteLine("\nSongs not found:");
            foreach (var song in _lookupErrors)
            {
                Console.WriteLine($"- {song}");
            }
        }

        if (_downloadErrors.Count > 0)
        {
            Console.WriteLine("\nSongs with download errors:");
            foreach (var url in _downloadErrors)
            {
                Console.WriteLine($"- {url}");
            }
        }

        Console.WriteLine($"\nTotal missing songs: {_lookupErrors.Count + _downloadErrors.Count}");
        Console.WriteLine("=========================");

        Console.ResetColor();
    }

    private async Task RunCommandAsync(string[] command)
    {
        try
        {
            _lookupErrors.Clear();
            _downloadErrors.Clear();

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "spotdl",
                Arguments = string.Join(" ", command),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.OutputDataReceived += (sender, args) => ProcessStandardOutput(args.Data!);
            // errors arent processed
            // process.ErrorDataReceived += (sender, args) => ProcessStandardError(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            PrintMissingSongs();

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
}