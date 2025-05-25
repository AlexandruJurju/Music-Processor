using Mediator;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
