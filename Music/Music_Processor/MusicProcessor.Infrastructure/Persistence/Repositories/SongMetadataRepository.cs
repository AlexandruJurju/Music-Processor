using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class SongMetadataRepository : ISongMetadataRepository
{
    private readonly ApplicationDbContext _context;

    public SongMetadataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<SongMetadata>> GetAllAsync()
    {
        return await GetAll().ToListAsync();
    }

    public async Task<IEnumerable<string>> GetSongTitlesAsync()
    {
        return await _context.Songs.Select(s => s.Name).ToListAsync();
    }

    public IQueryable<SongMetadata> GetAll()
    {
        return _context.Songs
            .Include(s => s.Genres)
            .ThenInclude(s => s.GenreCategories)
            .Include(s => s.Artists)
            .AsSplitQuery();
    }

    public async Task<SongMetadata?> GetByIdAsync(int id)
    {
        return await GetAll().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateAsync(SongMetadata songMetadata)
    {
        _context.Songs.Update(songMetadata);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var song = await _context.Songs.FindAsync(id);
        if (song != null)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddRangeAsync(List<SongMetadata> songsList)
    {
        await _context.Songs.AddRangeAsync(songsList);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(SongMetadata songMetadata)
    {
        await _context.Songs.AddAsync(songMetadata);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<SongMetadata> modifiedSongs)
    {
        _context.Songs.UpdateRange(modifiedSongs);
        await _context.SaveChangesAsync();
    }

    public async Task<SongMetadata?> GetByKeyAsync(string key)
    {
        return await _context.Songs
            .Include(s => s.MainArtist)
            .FirstOrDefaultAsync(s => (s.Name.ToLower().Trim() + " - " + s.MainArtist.Name.ToLower().Trim()) == key);
    }

    public async Task<Dictionary<string, SongMetadata>> GetAllSongsWithKeyAsync()
    {
        return await _context.Songs
            .Include(s => s.MainArtist)
            .ToDictionaryAsync(
                s => s.MainArtist.Name.ToLower().Trim() + " - " + s.Name.ToLower().Trim(),
                s => s
            );
    }
}