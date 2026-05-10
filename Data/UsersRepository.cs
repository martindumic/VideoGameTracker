using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class UsersRepository : IUsersRepository
{
    private readonly VideoGameTrackerDbContext _dbContext;

    public UsersRepository(VideoGameTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<User> GetAll() => _dbContext.Users
        .Include(u => u.GameEntries)
            .ThenInclude(e => e.Game)
        .ToList();

    public User? GetById(int id) => _dbContext.Users
        .Include(u => u.GameEntries)
            .ThenInclude(e => e.Game)
        .FirstOrDefault(u => u.Id == id);

    public List<User> Search(string? query)
    {
        var usersQuery = _dbContext.Users
            .Include(u => u.GameEntries)
                .ThenInclude(e => e.Game)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            usersQuery = usersQuery.Where(u =>
                EF.Functions.Like(u.Username ?? string.Empty, term) ||
                EF.Functions.Like(u.Email ?? string.Empty, term)
            );
        }

        return usersQuery.ToList();
    }

    public void Create(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }

    public bool Update(User user)
    {
        var existing = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
        if (existing == null)
        {
            return false;
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(user);
        _dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return false;
        }

        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();
        return true;
    }
}
