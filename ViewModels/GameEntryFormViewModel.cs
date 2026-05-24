using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoGameTracker.Models;

namespace VideoGameTracker.ViewModels;

public class GameEntryFormViewModel
{
    public string? FormContext { get; set; }

    public int? Id { get; set; }

    [Required(ErrorMessage = "Game is required.")]
    public int? GameId { get; set; }

    public string? GameTitle { get; set; }

    public string? UserId { get; set; }

    public string? Username { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public GameStatus? Status { get; set; }

    [Range(0, 10000, ErrorMessage = "Hours must be between 0 and 10000.")]
    public int HoursPlayed { get; set; }

    [Range(1, 100, ErrorMessage = "Score must be between 1 and 100.")]
    public int? ReviewScore { get; set; }

    [StringLength(1000, ErrorMessage = "Comment can be up to 1000 characters.")]
    public string? ReviewComment { get; set; }

    [Required(ErrorMessage = "Review date and time is required.")]
    public string? ReviewTimestamp { get; set; }

    public IEnumerable<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();

    public bool CanSelectUser { get; set; }
}
