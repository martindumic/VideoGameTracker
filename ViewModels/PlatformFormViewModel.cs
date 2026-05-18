using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoGameTracker.Models;

namespace VideoGameTracker.ViewModels;

public class PlatformFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(120, ErrorMessage = "Name can be up to 120 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Platform type is required.")]
    public PlatformType? Type { get; set; }

    public IEnumerable<SelectListItem> TypeOptions { get; set; } = new List<SelectListItem>();
}
