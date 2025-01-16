using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class StyleRepository : Repository<Style>, IStyleRepository
{
    public StyleRepository(ApplicationDbContext context) : base(context)
    {
    }
}