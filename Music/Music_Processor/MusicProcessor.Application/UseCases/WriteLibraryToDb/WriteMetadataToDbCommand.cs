using MediatR;

namespace MusicProcessor.Application.UseCases.WriteLibraryToDb;

public record WriteMetadataToDbCommand(string PlaylistPath) : IRequest;