using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameTracker.Models;

public class GameEntryScreenshot
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(GameEntry))]
    public int GameEntryId { get; set; }

    public virtual GameEntry? GameEntry { get; set; }

    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    public string? UploadedByUserId { get; set; }

    [StringLength(200)]
    public string? Caption { get; set; }
}
