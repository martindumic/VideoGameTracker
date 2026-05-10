using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class PlatformsRepository : IPlatformsRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public PlatformsRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Platform> GetAll() => _dbContext.Platforms
        .Include(p => p.Games)
        .ToList();

    public Platform? GetById(int id) => _dbContext.Platforms
        .Include(p => p.Games)
        .FirstOrDefault(p => p.Id == id);

    public List<Platform> Search(string? query)
    {
        var platformsQuery = _dbContext.Platforms
            .Include(p => p.Games)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            platformsQuery = platformsQuery.Where(p =>
                EF.Functions.Like(p.Name ?? string.Empty, term)
            );
        }

        return platformsQuery.ToList();
    }

    public void Create(Platform platform)
    {
        _dbContext.Platforms.Add(platform);
        _dbContext.SaveChanges();
    }

    public bool Update(Platform platform)
    {
        var existing = _dbContext.Platforms.FirstOrDefault(p => p.Id == platform.Id);
        if (existing == null)
        {
            return false;
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(platform);
        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var platform = _dbContext.Platforms.FirstOrDefault(p => p.Id == id);
        if (platform == null)
        {
            return false;
        }

        _dbContext.Platforms.Remove(platform);
        _dbContext.SaveChanges();
        return true;
    }
}
