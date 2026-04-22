using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class GamesController : Controller
    {
        private readonly GamesRepository _gamesRepository;

        public GamesController(GamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        public IActionResult Index()
        {
            var games = _gamesRepository.GetAll();
            return View(games);
        }

        public IActionResult Details(int id)
        {
            var game = _gamesRepository.GetById(id);
            if (game == null)
                return NotFound();
            return View(game);
        }
    }
}
