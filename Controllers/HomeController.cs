using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGameTracker.Data;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsersMockRepository _usersRepository;
        private readonly GamesMockRepository _gamesRepository;
        private readonly GameEntriesMockRepository _gameEntriesRepository;
        private readonly ReviewsMockRepository _reviewsRepository;

        public HomeController(
            UsersMockRepository usersRepository,
            GamesMockRepository gamesRepository,
            GameEntriesMockRepository gameEntriesRepository,
            ReviewsMockRepository reviewsRepository)
        {
            _usersRepository = usersRepository;
            _gamesRepository = gamesRepository;
            _gameEntriesRepository = gameEntriesRepository;
            _reviewsRepository = reviewsRepository;
        }

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

        [HttpPost]
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
                TempData["Error"] = "Korisnik ili igra nisu pronađeni!";
                return RedirectToAction("Index");
            }

            // Kreiraj Review
            var review = new Review
            {
                Id = _reviewsRepository.GetAll().Max(r => r.Id) + 1,
                User = user,
                Game = game,
                Score = score,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            // Kreiraj GameEntry
            var gameEntry = new GameEntry
            {
                Id = _gameEntriesRepository.GetAll().Max(e => e.Id) + 1,
                Game = game,
                User = user,
                Status = status,
                DateAdded = DateTime.Now,
                HoursPlayed = hoursPlayed,
                Review = review
            };

            // Dodaj u mock repozitorije (ovi su singleton pa će biti dostupni dalje)
            _gameEntriesRepository.GetAll().Add(gameEntry);
            _reviewsRepository.GetAll().Add(review);
            game.Reviews.Add(review);
            user.GameEntries.Add(gameEntry);

            TempData["Success"] = "Game entry je uspješno kreiran!";
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
