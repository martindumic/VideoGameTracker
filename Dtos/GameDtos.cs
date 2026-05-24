using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Dtos;

public class GameDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int ReleaseYear { get; set; }
    public string? Description { get; set; }
    public int DeveloperId { get; set; }
    public DeveloperSummaryDto? Developer { get; set; }
    public int AverageRating { get; set; }
    public List<GenreSummaryDto> Genres { get; set; } = new();
    public List<PlatformSummaryDto> Platforms { get; set; } = new();
}

public class GameCreateDto
{
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    [Range(1970, 2100)]
    public int ReleaseYear { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int? DeveloperId { get; set; }

    [Range(0, 100)]
    public int AverageRating { get; set; }

    public List<int> GenreIds { get; set; } = new();
    public List<int> PlatformIds { get; set; } = new();
}

public class GameUpdateDto : GameCreateDto
{
    [Required]
    public int Id { get; set; }
}
