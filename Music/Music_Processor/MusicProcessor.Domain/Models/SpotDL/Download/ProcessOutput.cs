using MusicProcessor.Domain.Enums;

namespace MusicProcessor.Domain.Models.SpotDL.Download;

public record ProcessOutput(
    string Data,
    OutputType Type
);