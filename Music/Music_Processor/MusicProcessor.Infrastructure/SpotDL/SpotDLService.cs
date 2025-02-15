using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Enums;
using MusicProcessor.Domain.Models.SpotDL.Download;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLService(ILogger<SpotDLService> logger) : ISpotDLService
{
    private const string LookupErrorPrefix = "LookupError: No results found for song:";
    private const string AudioProviderErrorPrefix = "AudioProviderError: YT-DLP download error";
    private const string YoutubeUrlPrefix = "https://music.youtube.com/watch?v=";
    private const string GeneratedException = "generated";

    public async IAsyncEnumerable<ProcessOutput> NewSyncAsync(string playlistUrl, string playlistDirPath)
    {
        Directory.CreateDirectory(playlistDirPath);

        var command = new[]
        {
            "sync",
            playlistUrl,
            "--output",
            playlistDirPath,
            "--save-file",
            Path.Combine(playlistDirPath, $"{playlistDirPath}.spotdl")
        };

        await foreach (var output in RunCommandAsync(command)) yield return output;
    }

    public async IAsyncEnumerable<ProcessOutput> UpdateSyncAsync(string playlistDirPath)
    {
        var syncFile = $"{playlistDirPath}.spotdl";
        var playlistName = Path.GetFileNameWithoutExtension(syncFile);

        if (!File.Exists(syncFile)) throw new FileNotFoundException($"Sync file not found for playlist {playlistName}");

        var command = new[]
        {
            "sync",
            syncFile,
            "--output",
            playlistDirPath
        };

        await foreach (var output in RunCommandAsync(command)) yield return output;
    }

    private ProcessOutput ProcessStandardOutput(string data, SyncSummary summary)
    {
        if (string.IsNullOrEmpty(data))
            return new ProcessOutput(string.Empty, OutputType.Success);


        if (IsStandardErrorOutput(data))
        {
            ProcessErrorData(data, summary);
            return new ProcessOutput(data.Trim(), OutputType.Error);
        }

        return new ProcessOutput(data.Trim(), OutputType.Success);
    }

    private bool IsStandardErrorOutput(ReadOnlySpan<char> data)
    {
        return data.Contains(LookupErrorPrefix, StringComparison.Ordinal) ||
               data.Contains(AudioProviderErrorPrefix, StringComparison.Ordinal) ||
               data.TrimStart().StartsWith(YoutubeUrlPrefix, StringComparison.Ordinal) ||
               data.Contains(GeneratedException, StringComparison.Ordinal);
    }

    private void ProcessErrorData(string data, SyncSummary summary)
    {
        if (data.Contains(LookupErrorPrefix, StringComparison.Ordinal))
        {
            var songName = data.Substring(LookupErrorPrefix.Length).Trim();
            summary.LookupErrors.Add(songName);
            logger.LogWarning("{Message}", $"Lookup error: {songName}");
        }
        else if (data.TrimStart().StartsWith(YoutubeUrlPrefix, StringComparison.Ordinal))
        {
            summary.DownloadErrors.Add(data.Trim());
            logger.LogWarning("{Message}", $"Download error: {data.Trim()}");
        }
        else if (data.Trim().Contains(GeneratedException, StringComparison.Ordinal))
        {
            var songName = data.Split(GeneratedException)[0];
            summary.DownloadErrors.Add(songName);
            logger.LogWarning("{Message}", $"Download error: {songName}");
        }
    }

    private async IAsyncEnumerable<ProcessOutput> RunCommandAsync(string[] command)
    {
        var summary = new SyncSummary();
        var outputQueue = new ConcurrentQueue<ProcessOutput>();
        var processingComplete = new TaskCompletionSource<bool>();

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "spotdl",
            Arguments = string.Join(" ", command),
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            EnvironmentVariables =
            {
                ["COLS"] = "500",
                ["COLUMNS"] = "500",
                ["LINES"] = "50"
            }
        };
        process.EnableRaisingEvents = true;

        // Handle standard output
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                var output = ProcessStandardOutput(args.Data.Trim(), summary);
                outputQueue.Enqueue(output);
            }
        };

        // Handle standard error
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                var output = ProcessStandardOutput(args.Data, summary);
                outputQueue.Enqueue(output);
            }
        };

        process.Exited += (sender, args) => processingComplete.SetResult(true);

        // Start the process
        process.Start();
        process.BeginOutputReadLine();

        // Monitor and yield outputs
        while (!processingComplete.Task.IsCompleted || !outputQueue.IsEmpty)
            if (outputQueue.TryDequeue(out var output))
                yield return output;

        // await Task.Delay(50);
        // Wait for process completion
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            logger.LogError($"spotdl process failed with exit code {process.ExitCode}");
            throw new Exception($"spotdl command failed with exit code {process.ExitCode}");
        }

        var processOutput = new ProcessOutput(
            $"\n=== Missing Songs Summary ===\n" +
            $"Lookup Errors: {summary.LookupErrors.Count}\n" +
            $"Download Errors: {summary.DownloadErrors.Count}\n" +
            $"Total Missing: {summary.TotalMissingSongs}",
            OutputType.Error
        );
        
        yield return processOutput;
    }
}