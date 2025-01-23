using MediatR;

namespace MusicProcessor.Application.UseCases.ClearPersistence;

public record ClearDbCommand() : IRequest;