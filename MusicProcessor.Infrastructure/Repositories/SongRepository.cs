using MusicProcessor.Domain;
using MusicProcessor.Domain.Abstractions.Persistence;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Session;

namespace MusicProcessor.Infrastructure.Repositories;

public class SongRepository(IDocumentStore documentStore) : ISongRepository
{
    public async Task<IEnumerable<Song>> GetAllAsync()
    {
        using IAsyncDocumentSession? session = documentStore.OpenAsyncSession();
        return await session.Query<Song>()
            .ToListAsync();
    }

    public async Task<string> AddAsync(Song song)
    {
        using IAsyncDocumentSession? session = documentStore.OpenAsyncSession();
        await session.StoreAsync(song);
        await session.SaveChangesAsync();
        return song.Id;
    }

    public async Task AddRangeAsync(IEnumerable<Song> songs)
    {
        await using BulkInsertOperation? bulkInsert = documentStore.BulkInsert();

        foreach (Song song in songs)
        {
            await bulkInsert.StoreAsync(song);
        }
    }
}
