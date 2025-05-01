using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.SongsMetadata;
using MusicProcessor.Persistence.Common;

namespace MusicProcessor.Infrastructure.Repositories;

public class SongMetadataRepository(ApplicationDbContext dbContext) : ISongMetadataRepository
{
    public async Task<IEnumerable<string>> GetSongTitlesAsync()
    {
        return await dbContext.Songs.Select(s => s.Name).ToListAsync();
    }

    public async Task<List<SongMetadata>> GetAllAsync()
    {
        return await dbContext.Songs
            .Include(s => s.Genres)
            .ThenInclude(s => s.GenreCategories)
            .Include(s => s.Artists)
            .ToListAsync();
    }

    public async Task<SongMetadata?> GetByIdAsync(int id)
    {
        return await dbContext.Songs
            .Include(s => s.Genres)
            .ThenInclude(s => s.GenreCategories)
            .Include(s => s.Artists)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(SongMetadata songMetadata)
    {
        dbContext.Songs.Update(songMetadata);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        SongMetadata? song = await dbContext.Songs.FindAsync(id);
        if (song != null)
        {
            dbContext.Songs.Remove(song);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task AddRangeAsync(List<SongMetadata> songsList)
    {
        await dbContext.Songs.AddRangeAsync(songsList);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(SongMetadata songMetadata)
    {
        await dbContext.Songs.AddAsync(songMetadata);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<SongMetadata> modifiedSongs)
    {
        dbContext.Songs.UpdateRange(modifiedSongs);
        await dbContext.SaveChangesAsync();
    }

    public async Task<SongMetadata?> GetByKeyAsync(string key)
    {
        return await dbContext.Songs
            .Include(s => s.MainArtist)
            .FirstOrDefaultAsync(s => s.Name.ToLower().Trim() + " - " + s.MainArtist.Name.ToLower().Trim() == key);
    }

    public async Task<Dictionary<string, SongMetadata>> GetAllSongsWithKeyAsync()
    {
        return await dbContext.Songs
            .Include(s => s.MainArtist)
            .AsNoTracking()
            .ToDictionaryAsync(
                s => s.MainArtist.Name.ToLower().Trim() + " - " + s.Name.ToLower().Trim(),
                s => s
            );
    }
}
