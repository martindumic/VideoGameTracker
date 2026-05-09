using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameTracker.Models;

public class Game
{
    [Key]
    public int Id { get; set; }

    public string? Title { get; set; }

    public int ReleaseYear { get; set; }

    public string? Description { get; set; }

    [ForeignKey(nameof(Developer))]
    public int DeveloperId { get; set; }

    public virtual Developer? Developer { get; set; }

    public int AverageRating { get; set; }

    public virtual ICollection<Genre> Genres { get; set; }

    public virtual ICollection<Platform> Platforms { get; set; }

    public virtual ICollection<GameEntry> GameEntries { get; set; }

    public Game()
    {
        Genres = new List<Genre>();
        Platforms = new List<Platform>();
        GameEntries = new List<GameEntry>();
    }
}
