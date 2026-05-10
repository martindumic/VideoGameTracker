using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IGenresRepository
{
    List<Genre> GetAll();
    Genre? GetById(int id);
    List<Genre> Search(string? query);
    void Create(Genre genre);
    bool Update(Genre genre);
    bool Delete(int id);
}
