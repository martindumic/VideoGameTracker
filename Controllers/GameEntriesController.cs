using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class GameEntriesController : Controller
    {
        private readonly GameEntriesMockRepository _gameEntriesRepository;

        public GameEntriesController(GameEntriesMockRepository gameEntriesRepository)
        {
            _gameEntriesRepository = gameEntriesRepository;
        }

        public IActionResult Index()
        {
            var gameEntries = _gameEntriesRepository.GetAll();
            return View(gameEntries);
        }

        public IActionResult Details(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
                return NotFound();
            return View(gameEntry);
        }
    }
}
