using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Songs.FixSongMetadata;

public class FixSongMetadataCommandHandler(
    ISongRepository songRepository,
    IStyleMappingRepository genreMappingRepository
) : ICommandHandler<FixSongMetadataCommand>
{
    public async ValueTask<Result> Handle(FixSongMetadataCommand request, CancellationToken cancellationToken)
    {
        if (!genreMappingRepository.GetAll().Any())
        {
            var toAdd = new List<GenreMappings>
            {
                new()
                {
                    Name = "rock", Styles =
                    [
                        "country rock",
                        "electric rock",
                        "electronic rock",
                        "christian rock"
                    ]
                },
                new()
                {
                    Name = "synthwave", Styles =
                    [
                        "darkwave",
                        "chillwave",
                        "vaporwave",
                        "synth pop"
                    ]
                },
                new()
                {
                    Name =
                        "dubstep",
                    Styles =
                    [
                        "chillstep"
                    ]
                },
                new()
                {
                    Name = "phonk", Styles =
                    [
                        "drift phonk"
                    ]
                },
                new()
                {
                    Name = "house", Styles =
                    [
                        "electronic house",
                        "slap house",
                        "future house"
                    ]
                },
                new()
                {
                    Name = "pop",
                    Styles = ["synth pop"]
                },
            };

            foreach (GenreMappings genreMapping in toAdd)
            {
                genreMappingRepository.Add(genreMapping);
            }
        }

        IEnumerable<Song> songs = songRepository.GetAll();

        IEnumerable<GenreMappings> genreMappings = genreMappingRepository.GetAll().ToList();

        foreach (Song song in songs)
        {
            foreach (GenreMappings genreMapping in genreMappings)
            {
                foreach (string style in genreMapping.Styles)
                {
                    if (song.Styles.Contains(style))
                    {
                        if (!song.Genres.Contains(genreMapping.Name))
                        {
                            song.Genres.Add(genreMapping.Name);
                        }

                        songRepository.Update(song);
                    }

                    if (song.Styles.Contains(genreMapping.Name))
                    {
                        song.Styles.Remove(genreMapping.Name);
                        if (!song.Genres.Contains(genreMapping.Name))
                        {
                            song.Genres.Add(genreMapping.Name);
                        }

                        songRepository.Update(song);
                    }
                }
            }
        }

        await Task.CompletedTask;

        return Result.Success();
    }
}
