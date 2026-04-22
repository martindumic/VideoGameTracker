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
    public int UserId { get; set; }

    public virtual User? User { get; set; }

    public GameStatus Status { get; set; }

    public DateTime DateAdded { get; set; }

    public int HoursPlayed { get; set; }

    [ForeignKey(nameof(Review))]
    public int? ReviewId { get; set; }

    public virtual Review? Review { get; set; }
}
