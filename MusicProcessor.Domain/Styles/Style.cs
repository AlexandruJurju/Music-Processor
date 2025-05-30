using MusicProcessor.Domain.Genres;

namespace MusicProcessor.Domain.Styles;

public class Style
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool SoftDeleted { get; set; }
    public IEnumerable<Genre> Genres { get; set; }

    private Style(string name, bool softDeleted)
    {
        Name = name;
        SoftDeleted = softDeleted;
        Genres = new List<Genre>();
    }

    public static Style Create(string name, bool softDeleted)
    {
        return new Style(name, softDeleted);
    }

    protected Style() { }
}
