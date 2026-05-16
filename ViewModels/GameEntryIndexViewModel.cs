using VideoGameTracker.Models;

namespace VideoGameTracker.ViewModels;

public class GameEntryIndexViewModel
{
    public List<GameEntry> Entries { get; set; } = new();

    public GameEntryFormViewModel Form { get; set; } = new();
}
