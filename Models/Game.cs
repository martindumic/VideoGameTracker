namespace VideoGameTracker.Models;

public class Game
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int ReleaseYear { get; set; }

    public string? Description { get; set; }

    public Developer? Developer { get; set; }

    public int AverageRating { get; set; }

    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }

    public List<Review> Reviews { get; set; }

    public Game()
    {
        Genres = new List<Genre>();
        Platforms = new List<Platform>();
        Reviews = new List<Review>();
    }
}
