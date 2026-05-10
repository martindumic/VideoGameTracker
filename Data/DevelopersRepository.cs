using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class DevelopersRepository : IDevelopersRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public DevelopersRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Developer> GetAll() => _dbContext.Developers
        .Include(d => d.Games)
        .ToList();

    public Developer? GetById(int id) => _dbContext.Developers
        .Include(d => d.Games)
        .FirstOrDefault(d => d.Id == id);

    public List<Developer> Search(string? query)
    {
        var developersQuery = _dbContext.Developers
            .Include(d => d.Games)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            developersQuery = developersQuery.Where(d =>
                EF.Functions.Like(d.Name ?? string.Empty, term) ||
                EF.Functions.Like(d.Country ?? string.Empty, term) ||
                EF.Functions.Like(d.Description ?? string.Empty, term)
            );
        }

        return developersQuery.ToList();
    }

    public void Create(Developer developer)
    {
        _dbContext.Developers.Add(developer);
        _dbContext.SaveChanges();
    }

    public bool Update(Developer developer)
    {
        var existing = _dbContext.Developers.FirstOrDefault(d => d.Id == developer.Id);
        if (existing == null)
        {
            return false;
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(developer);
        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var developer = _dbContext.Developers.FirstOrDefault(d => d.Id == id);
        if (developer == null)
        {
            return false;
        }

        _dbContext.Developers.Remove(developer);
        _dbContext.SaveChanges();
        return true;
    }
}
