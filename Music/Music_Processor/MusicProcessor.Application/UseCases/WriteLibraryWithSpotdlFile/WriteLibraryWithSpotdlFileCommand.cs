using MediatR;

namespace MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

public sealed record WriteLibraryWithSpotdlFileCommand(string PlaylistPath) : IRequest;