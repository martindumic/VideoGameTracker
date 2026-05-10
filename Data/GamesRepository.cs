using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class GamesRepository : IGamesRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public GamesRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Game> GetAll() => _dbContext.Games
        .Include(g => g.Developer)
        .Include(g => g.Genres)
        .Include(g => g.Platforms)
        .ToList();

    public Game? GetById(int id) => _dbContext.Games
        .Include(g => g.Developer)
        .Include(g => g.Genres)
        .Include(g => g.Platforms)
        .FirstOrDefault(g => g.Id == id);

    public List<Game> Search(string? query)
    {
        var gamesQuery = _dbContext.Games
            .Include(g => g.Developer)
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            gamesQuery = gamesQuery.Where(g =>
                EF.Functions.Like(g.Title ?? string.Empty, term) ||
                EF.Functions.Like(g.Description ?? string.Empty, term) ||
                (g.Developer != null && EF.Functions.Like(g.Developer.Name ?? string.Empty, term)) ||
                g.Genres.Any(genre => EF.Functions.Like(genre.Name ?? string.Empty, term)) ||
                g.Platforms.Any(platform => EF.Functions.Like(platform.Name ?? string.Empty, term))
            );
        }

        return gamesQuery.ToList();
    }

    public void Create(Game game)
    {
        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();
    }

    public bool Update(Game game)
    {
        var existing = _dbContext.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .FirstOrDefault(g => g.Id == game.Id);

        if (existing == null)
        {
            return false;
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(game);

        if (game.Genres != null)
        {
            existing.Genres.Clear();
            foreach (var genre in game.Genres)
            {
                var trackedGenre = _dbContext.Genres.Find(genre.Id);
                if (trackedGenre != null)
                {
                    existing.Genres.Add(trackedGenre);
                }
            }
        }

        if (game.Platforms != null)
        {
            existing.Platforms.Clear();
            foreach (var platform in game.Platforms)
            {
                var trackedPlatform = _dbContext.Platforms.Find(platform.Id);
                if (trackedPlatform != null)
                {
                    existing.Platforms.Add(trackedPlatform);
                }
            }
        }

        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var game = _dbContext.Games.FirstOrDefault(g => g.Id == id);
        if (game == null)
        {
            return false;
        }

        _dbContext.Games.Remove(game);
        _dbContext.SaveChanges();
        return true;
    }
}
