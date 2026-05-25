using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GameEntryScreenshotsRepository : IGameEntryScreenshotsRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public GameEntryScreenshotsRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<GameEntryScreenshot> GetByGameEntryId(int gameEntryId)
    {
        return _dbContext.GameEntryScreenshots
            .Where(s => s.GameEntryId == gameEntryId)
            .OrderByDescending(s => s.UploadedAt)
            .ToList();
    }

    public GameEntryScreenshot? GetById(int id)
    {
        return _dbContext.GameEntryScreenshots
            .Include(s => s.GameEntry)
            .FirstOrDefault(s => s.Id == id);
    }

    public void Create(GameEntryScreenshot screenshot)
    {
        _dbContext.GameEntryScreenshots.Add(screenshot);
        _dbContext.SaveChanges();
    }

    public bool Delete(GameEntryScreenshot screenshot)
    {
        _dbContext.GameEntryScreenshots.Remove(screenshot);
        _dbContext.SaveChanges();
        return true;
    }
}
