using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GameEntriesRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public GameEntriesRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<GameEntry> GetAll() => _dbContext.GameEntries
        .Include(e => e.Game)
        .Include(e => e.User)
        .ToList();

    public GameEntry? GetById(int id) => _dbContext.GameEntries
        .Include(e => e.Game)
        .Include(e => e.User)
        .FirstOrDefault(e => e.Id == id);

    public void Add(GameEntry gameEntry)
    {
        _dbContext.GameEntries.Add(gameEntry);
        _dbContext.SaveChanges();
    }
}
