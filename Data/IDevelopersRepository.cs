using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IDevelopersRepository
{
    List<Developer> GetAll();
    Developer? GetById(int id);
    List<Developer> Search(string? query);
    void Create(Developer developer);
    bool Update(Developer developer);
    bool Delete(int id);
}
