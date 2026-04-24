using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    [Route("game-entries")]
    public class GameEntriesController : Controller
    {
        private readonly GameEntriesRepository _gameEntriesRepository;

        public GameEntriesController(GameEntriesRepository gameEntriesRepository)
        {
            _gameEntriesRepository = gameEntriesRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var gameEntries = _gameEntriesRepository.GetAll();
            return View(gameEntries);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
                return NotFound();
            return View(gameEntry);
        }
    }
}
