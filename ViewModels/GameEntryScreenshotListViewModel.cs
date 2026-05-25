using VideoGameTracker.Models;

namespace VideoGameTracker.ViewModels;

public class GameEntryScreenshotListViewModel
{
    public int GameEntryId { get; set; }
    public bool CanManage { get; set; }
    public bool UseSlideshow { get; set; }
    public List<GameEntryScreenshot> Screenshots { get; set; } = new();
}
