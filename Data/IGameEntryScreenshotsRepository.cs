using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IGameEntryScreenshotsRepository
{
    List<GameEntryScreenshot> GetByGameEntryId(int gameEntryId);
    GameEntryScreenshot? GetById(int id);
    void Create(GameEntryScreenshot screenshot);
    bool Delete(GameEntryScreenshot screenshot);
}
