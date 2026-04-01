namespace VideoGameTracker.Models;

public class GameEntry
{
    public int Id { get; set; }

    public Game? Game { get; set; }

    public User? User { get; set; }

    public GameStatus Status { get; set; }

    public DateTime DateAdded { get; set; }

    public int HoursPlayed { get; set; }

    public Review? Review { get; set; }
}
