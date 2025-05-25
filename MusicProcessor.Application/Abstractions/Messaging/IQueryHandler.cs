using MusicProcessor.Domain.Abstractions.Result;
using Mediator;

namespace MusicProcessor.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
