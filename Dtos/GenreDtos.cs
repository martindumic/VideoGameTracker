using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Dtos;

public class GenreDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class GenreCreateDto
{
    [Required]
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class GenreUpdateDto : GenreCreateDto
{
    [Required]
    public int Id { get; set; }
}
