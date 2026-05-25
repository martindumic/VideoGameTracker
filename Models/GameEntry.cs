using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameTracker.Models;

public class GameEntry
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    public virtual Game? Game { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = string.Empty;

    public virtual AppUser? User { get; set; }

    public GameStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public int HoursPlayed { get; set; }

    public int? ReviewScore { get; set; }

    public string? ReviewComment { get; set; }

    public virtual ICollection<GameEntryScreenshot> Screenshots { get; set; } = new List<GameEntryScreenshot>();

}
