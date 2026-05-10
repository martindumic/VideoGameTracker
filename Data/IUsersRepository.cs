using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IUsersRepository
{
    List<User> GetAll();
    User? GetById(int id);
    List<User> Search(string? query);
    void Create(User user);
    bool Update(User user);
    bool Delete(int id);
}
