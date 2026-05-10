using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IGamesRepository
{
    List<Game> GetAll();
    Game? GetById(int id);
    List<Game> Search(string? query);
    void Create(Game game);
    bool Update(Game game);
    bool Delete(int id);
}
