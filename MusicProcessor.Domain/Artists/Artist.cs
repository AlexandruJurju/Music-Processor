namespace MusicProcessor.Domain.Artists;

public class Artist
{
    public int Id { get; }
    public string Name { get; set; }

    private Artist(string name)
    {
        Name = name;
    }

    public static Artist Create(string name)
    {
        var artist = new Artist(name);

        return artist;
    }

    protected Artist() { }
}
