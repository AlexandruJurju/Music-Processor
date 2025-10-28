using MusicProcessor.Domain;
using MusicProcessor.Infrastructure.Contracts;
using Spectre.Console.Cli;

namespace MusicProcessor.Features.ExportMetadata;

public sealed class ExportMetadataCommand(
    IFileService fileService,
    IAudioService audioService,
    ISongRepository songRepository
) : AsyncCommand<ExportMetadataCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var files = fileService.GetAllSongFilePaths();

        List<Song> physicalSongs = [];
        foreach (var file in files)
        {
            var song = await audioService.ReadMetadataAsync(file);
            physicalSongs.Add(song);
        }

        var songs = await songRepository.GetAllAsync();
        var enumerable = songs as Song[] ?? songs.ToArray();

        foreach (var physicalSong in physicalSongs)
        {
            if (enumerable.All(s => s.GetSongKey() != physicalSong.GetSongKey()))
            {
                Console.WriteLine(physicalSong.GetSongKey());
            }
        }

        return 0;
    }
}