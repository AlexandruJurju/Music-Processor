using Mediator;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
