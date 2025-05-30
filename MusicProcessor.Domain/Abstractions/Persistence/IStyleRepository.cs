using MusicProcessor.Domain.Styles;

namespace MusicProcessor.Domain.Abstractions.Persistence;

public interface IStyleRepository
{
    Task BulkInsertAsync(IEnumerable<Style> styles);
}
