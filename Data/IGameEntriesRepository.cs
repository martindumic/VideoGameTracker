using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IGameEntriesRepository
{
    List<GameEntry> GetAll();
    GameEntry? GetById(int id);
    List<GameEntry> Search(string? query);
    void Create(GameEntry gameEntry);
    bool Update(GameEntry gameEntry);
    bool Delete(int id);
}
