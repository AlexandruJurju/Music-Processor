using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class SongRepository : ISongRepository
{
    private readonly ApplicationDbContext _context;

    public SongRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(Song song)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Song?> GetByIdAsync(int id)
    {
        return await _context.Songs
            .Include(s => s.Genres)
            .Include(s => s.Styles)
            .Include(s => s.Artists)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Song>> GetAllAsync()
    {
        return await _context.Songs
            .Include(s => s.Genres)
            .Include(s => s.Styles)
            .Include(s => s.Artists)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetSongTitlesAsync()
    {
        return await _context.Songs.Select(s => s.Title).ToListAsync();
    }

    public async Task AddRangeAsync(List<Song> songsToAdd)
    {
        await _context.Songs.AddRangeAsync(songsToAdd);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Song song)
    {
        var existingSong = await _context.Songs
            .Include(s => s.Genres)
            .Include(s => s.Styles)
            .Include(s => s.Artists)
            .FirstOrDefaultAsync(s => s.Id == song.Id);

        if (existingSong == null)
        {
            throw new KeyNotFoundException($"Song with ID {song.Id} not found");
        }

        // Update other properties
        existingSong.Title = song.Title;
        existingSong.Album = song.Album;
        existingSong.Year = song.Year;
        existingSong.Comment = song.Comment;
        existingSong.TrackNumber = song.TrackNumber;
        existingSong.Duration = song.Duration;
        existingSong.FileType = song.FileType;

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

    public IQueryable<Song> GetAll()
    {
        return _context.Songs
            .Include(s => s.Genres)
            .Include(s => s.Styles)
            .Include(s => s.Artists);
    }
}