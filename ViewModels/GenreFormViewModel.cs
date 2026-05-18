using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.ViewModels;

public class GenreFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(120, ErrorMessage = "Name can be up to 120 characters.")]
    public string? Name { get; set; }

    [StringLength(2000, ErrorMessage = "Description can be up to 2000 characters.")]
    public string? Description { get; set; }
}
