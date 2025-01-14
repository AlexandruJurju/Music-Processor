namespace Music_Processor.Interfaces;

public interface ISpotDLService
{
    Task NewSyncAsync(string playlistUrl, string playlistName, string baseDir);
    Task UpdateSyncAsync(string playlistName, string baseDir);
}