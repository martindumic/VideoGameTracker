using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Dtos;

public class DeveloperDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    public DateTime Founded { get; set; }
    public string? Description { get; set; }
}

public class DeveloperCreateDto
{
    [Required]
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(200)]
    public string? Country { get; set; }

    public DateTime Founded { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class DeveloperUpdateDto : DeveloperCreateDto
{
    [Required]
    public int Id { get; set; }
}
