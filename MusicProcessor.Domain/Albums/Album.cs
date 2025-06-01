using MusicProcessor.Domain.Artists;

namespace MusicProcessor.Domain.Albums;

public class Album
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Artist MainArtist { get; set; }

    private Album(string name, Artist mainArtist)
    {
        Name = name;
        MainArtist = mainArtist;
    }


    public static Album Create(string name, Artist mainArtist)
    {
        return new Album(name, mainArtist);
    }

    protected Album() { }
}
