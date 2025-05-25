using LiteDB;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Infrastructure.Repositories;

public class GenreMappingRepository : IStyleMappingRepository
{
    private const string CollectionName = "genre_mappings";
    private readonly ILiteCollection<GenreMappings> _collection;

    public GenreMappingRepository(ILiteDatabase liteDatabase)
    {
        _collection = liteDatabase.GetCollection<GenreMappings>(CollectionName);
        _collection.EnsureIndex(x => x.Name);
    }

    public void Add(GenreMappings genreMappings)
    {
        _collection.Insert(genreMappings);
    }

    public IEnumerable<GenreMappings> GetAll()
    {
        return _collection.FindAll();
    }
}
