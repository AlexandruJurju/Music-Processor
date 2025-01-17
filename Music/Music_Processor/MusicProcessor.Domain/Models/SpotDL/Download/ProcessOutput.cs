using MusicProcessor.Domain.Enums;

namespace MusicProcessor.Domain.Models.SpotDL.Download;

public sealed record ProcessOutput(
    string Data,
    OutputType Type
);