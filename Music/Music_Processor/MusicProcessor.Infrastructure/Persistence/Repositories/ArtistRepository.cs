using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class ArtistRepository : Repository<Artist>, IArtistRepository
{
    public ArtistRepository(ApplicationDbContext context) : base(context)
    {
    }
}