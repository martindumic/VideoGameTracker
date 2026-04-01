namespace VideoGameTracker.Models;

public class Platform
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public PlatformType Type { get; set; }

    public List<Game> Games { get; set; }

    public Platform()
    {
        Games = new List<Game>();
    }
}
