using MusicProcessor.Domain.Models.SpotDL;
using MusicProcessor.Domain.Models.SpotDL.Download;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface ISpotDLService
{
    IAsyncEnumerable<ProcessOutput> NewSyncAsync(string playlistUrl, string playlistDirPath);
    IAsyncEnumerable<ProcessOutput> UpdateSyncAsync(string playlistDirPath);
}