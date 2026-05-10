using VideoGameTracker.Models;

namespace VideoGameTracker.Data;

public interface IPlatformsRepository
{
    List<Platform> GetAll();
    Platform? GetById(int id);
    List<Platform> Search(string? query);
    void Create(Platform platform);
    bool Update(Platform platform);
    bool Delete(int id);
}
