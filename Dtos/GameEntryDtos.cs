using System.ComponentModel.DataAnnotations;
using VideoGameTracker.Models;

namespace VideoGameTracker.Dtos;

public class GameEntryDto
{
    public int Id { get; set; }
    public GameSummaryDto? Game { get; set; }
    public UserSummaryDto? User { get; set; }
    public GameStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int HoursPlayed { get; set; }
    public int? ReviewScore { get; set; }
    public string? ReviewComment { get; set; }
}

public class GameEntryCreateDto
{
    [Required]
    public int? GameId { get; set; }

    public string? UserId { get; set; }

    [Required]
    public GameStatus? Status { get; set; }

    [Range(0, 10000)]
    public int HoursPlayed { get; set; }

    [Range(1, 100)]
    public int? ReviewScore { get; set; }

    [StringLength(1000)]
    public string? ReviewComment { get; set; }

    public DateTime? CreatedAt { get; set; }
}

public class GameEntryUpdateDto : GameEntryCreateDto
{
    [Required]
    public int Id { get; set; }
}
