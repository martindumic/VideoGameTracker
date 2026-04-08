using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public class UsersMockRepository
{
    private readonly List<User> _users;

    public UsersMockRepository()
    {
        _users = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "GamerPro",
                Email = "gamerpro@email.com",
                Password = "hashed_password_1",
                RegisteredAt = new DateTime(2020, 3, 15),
                GameEntries = new List<GameEntry>()
            },
            new User
            {
                Id = 2,
                Username = "RPGFanatic",
                Email = "rpgfan@email.com",
                Password = "hashed_password_2",
                RegisteredAt = new DateTime(2021, 7, 22),
                GameEntries = new List<GameEntry>()
            },
            new User
            {
                Id = 3,
                Username = "ActionJunkie",
                Email = "actionjunkie@email.com",
                Password = "hashed_password_3",
                RegisteredAt = new DateTime(2019, 11, 10),
                GameEntries = new List<GameEntry>()
            }
        };
    }

    public List<User> GetAll() => _users;

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);
}
