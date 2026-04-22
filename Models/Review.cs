using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameTracker.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual User? User { get; set; }

    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    public virtual Game? Game { get; set; }

    public int Score { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}
