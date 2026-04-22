using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class UsersRepository
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
}
