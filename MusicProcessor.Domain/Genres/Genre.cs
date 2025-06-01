namespace MusicProcessor.Domain.Genres;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; private set; }

    private Genre(string name)
    {
        Name = name;
    }

    public static Genre Create(string name)
    {
        return new Genre(name);
    }

    protected Genre() { }
}
