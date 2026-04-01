namespace VideoGameTracker.Models;

public class Genre
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public List<Game> Games { get; set; }

    public Genre()
    {
        Games = new List<Game>();
    }
}
