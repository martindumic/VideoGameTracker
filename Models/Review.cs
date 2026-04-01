namespace VideoGameTracker.Models;

public class Review
{
    public int Id { get; set; }

    public User? User { get; set; }

    public Game? Game { get; set; }

    public int Score { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}
