using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.Application.UseCases.ClearPersistence;

public class ClearDbCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ClearDbCommand>
{
    public async Task Handle(ClearDbCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.ClearDatabaseAsync();
    }
}