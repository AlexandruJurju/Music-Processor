using LiteDB;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Infrastructure.Repositories;

public class SongRepository : ISongRepository
{
    private const string CollectionName = "songs";
    private readonly ILiteCollection<Song> _collection;

    public SongRepository(ILiteDatabase liteDatabase)
    {
        _collection = liteDatabase.GetCollection<Song>(CollectionName);
        _collection.EnsureIndex(x => x.Id);
    }

    public IEnumerable<Song> GetAll()
    {
        return _collection.FindAll();
    }

    public Dictionary<string, Song> GetAllWithKey()
    {
        return _collection.FindAll()
            .ToDictionary(song => song.Key);
    }

    public Guid Add(Song song)
    {
        BsonValue? id = _collection.Insert(song);
        return id.AsGuid;
    }

    public bool Update(Song song)
    {
        return _collection.Update(song);
    }

    public Song? GetById(Guid id)
    {
        return _collection.FindById(id);
    }

    public bool Delete(Guid id)
    {
        return _collection.Delete(id);
    }

    public long Count()
    {
        return _collection.Count();
    }
}
