using System.ComponentModel.DataAnnotations;
using VideoGameTracker.Models;

namespace VideoGameTracker.Dtos;

public class PlatformDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
}

public class PlatformCreateDto
{
    [Required]
    [StringLength(200)]
    public string? Name { get; set; }

    [Required]
    public PlatformType? Type { get; set; }
}

public class PlatformUpdateDto : PlatformCreateDto
{
    [Required]
    public int Id { get; set; }
}
