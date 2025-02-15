using MusicProcessor.Domain.Models.SpotDL.Download;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISpotDLService
{
    IAsyncEnumerable<ProcessOutput> NewSyncAsync(string playlistUrl, string playlistDirPath);
    IAsyncEnumerable<ProcessOutput> UpdateSyncAsync(string playlistDirPath);
}