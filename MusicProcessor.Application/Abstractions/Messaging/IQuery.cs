using MusicProcessor.Domain.Abstractions.Result;
using Mediator;

namespace MusicProcessor.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
