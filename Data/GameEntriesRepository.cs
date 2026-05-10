using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GameEntriesRepository : IGameEntriesRepository
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

    public List<GameEntry> Search(string? query)
    {
        var entriesQuery = _dbContext.GameEntries
            .Include(e => e.Game)
            .Include(e => e.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            entriesQuery = entriesQuery.Where(e =>
                (e.Game != null && EF.Functions.Like(e.Game.Title ?? string.Empty, term)) ||
                (e.User != null && EF.Functions.Like(e.User.Username ?? string.Empty, term)) ||
                EF.Functions.Like(e.ReviewComment ?? string.Empty, term)
            );
        }

        return entriesQuery.ToList();
    }

    public void Create(GameEntry gameEntry)
    {
        _dbContext.GameEntries.Add(gameEntry);
        _dbContext.SaveChanges();
    }

    public void Add(GameEntry gameEntry)
    {
        Create(gameEntry);
    }

    public bool Update(GameEntry gameEntry)
    {
        var existing = _dbContext.GameEntries.FirstOrDefault(e => e.Id == gameEntry.Id);
        if (existing == null)
        {
            return false;
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(gameEntry);
        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var entry = _dbContext.GameEntries.FirstOrDefault(e => e.Id == id);
        if (entry == null)
        {
            return false;
        }

        _dbContext.GameEntries.Remove(entry);
        _dbContext.SaveChanges();
        return true;
    }
}
