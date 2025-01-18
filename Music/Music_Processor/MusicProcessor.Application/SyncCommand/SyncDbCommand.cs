using MediatR;

namespace MusicProcessor.Application.SyncCommand;

public record SyncDbCommand(string PlaylistPath) : IRequest;