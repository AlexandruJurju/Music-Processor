using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class SongRepository : Repository<Song>, ISongRepository
{
    public SongRepository(ApplicationDbContext context) : base(context)
    {
    }
}