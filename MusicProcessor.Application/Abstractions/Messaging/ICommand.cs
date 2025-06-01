using Mediator;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
