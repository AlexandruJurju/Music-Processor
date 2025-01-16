using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(ApplicationDbContext context) : base(context)
    {
    }
}