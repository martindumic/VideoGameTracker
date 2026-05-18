using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VideoGameTracker.ViewModels;

public class GameFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title can be up to 200 characters.")]
    public string? Title { get; set; }

    [Range(1970, 2100, ErrorMessage = "Release year must be between 1970 and 2100.")]
    public int ReleaseYear { get; set; }

    [StringLength(2000, ErrorMessage = "Description can be up to 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Developer is required.")]
    public int? DeveloperId { get; set; }

    public string? DeveloperName { get; set; }

    [Range(0, 100, ErrorMessage = "Average rating must be between 0 and 100.")]
    public int AverageRating { get; set; }

    [MinLength(1, ErrorMessage = "Select at least one genre.")]
    public List<int> SelectedGenreIds { get; set; } = new();

    [MinLength(1, ErrorMessage = "Select at least one platform.")]
    public List<int> SelectedPlatformIds { get; set; } = new();

    public IEnumerable<SelectListItem> GenreOptions { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> PlatformOptions { get; set; } = new List<SelectListItem>();
}
