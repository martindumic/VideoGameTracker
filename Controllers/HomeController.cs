using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGameTracker.Data;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly GamesRepository _gamesRepository;
        private readonly GameEntriesRepository _gameEntriesRepository;

        public HomeController(
            UsersRepository usersRepository,
            GamesRepository gamesRepository,
            GameEntriesRepository gameEntriesRepository)
        {
            _usersRepository = usersRepository;
            _gamesRepository = gamesRepository;
            _gameEntriesRepository = gameEntriesRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            // Provjeri dali je korisnik prijavljen
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _usersRepository.GetById(userId.Value);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Login");
            }

            // Pripremi podatke za view
            var games = _gamesRepository.GetAll();
            var statuses = Enum.GetValues(typeof(GameStatus)).Cast<GameStatus>().ToList();

            // Kreiraj ViewModel ili proslijedi kroz ViewBag
            ViewBag.User = user;
            ViewBag.Games = games;
            ViewBag.Statuses = statuses;

            return View();
        }

        [HttpPost("create-game-entry")]
        public IActionResult CreateGameEntry(int gameId, GameStatus status, int hoursPlayed, string comment, int score)
        {
            // Provjeri dali je korisnik prijavljen
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _usersRepository.GetById(userId.Value);
            var game = _gamesRepository.GetById(gameId);

            if (user == null || game == null)
            {
                TempData["Error"] = "User or game was not found!";
                return RedirectToAction("Index");
            }

            // Kreiraj GameEntry
            var gameEntry = new GameEntry
            {
                GameId = game.Id,
                UserId = user.Id,
                Status = status,
                CreatedAt = DateTime.Now,
                HoursPlayed = hoursPlayed,
                ReviewScore = score,
                ReviewComment = string.IsNullOrWhiteSpace(comment) ? null : comment
            };
            _gameEntriesRepository.Add(gameEntry);

            TempData["Success"] = "Game entry was created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
