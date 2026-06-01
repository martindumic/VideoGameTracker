using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGameTracker.Data;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers
{
    [Route("home")]
    public class HomeController : BaseController
    {
        private readonly GamesRepository _gamesRepository;
        private readonly GameEntriesRepository _gameEntriesRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            GamesRepository gamesRepository,
            GameEntriesRepository gameEntriesRepository,
            UserManager<AppUser> userManager,
            ILogger<HomeController> logger)
            : base(userManager)
        {
            _gamesRepository = gamesRepository;
            _gameEntriesRepository = gameEntriesRepository;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("")]
        [HttpGet("/")]
        public IActionResult Index()
        {
            // Pripremi podatke za view
            var games = _gamesRepository.GetAll();
            var statuses = Enum.GetValues(typeof(GameStatus)).Cast<GameStatus>().ToList();

            // Kreiraj ViewModel ili proslijedi kroz ViewBag
            ViewBag.Games = games;
            ViewBag.Statuses = statuses;

            return View();
        }

        [Authorize]
        [HttpPost("create-game-entry")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGameEntry(int gameId, GameStatus status, int hoursPlayed, string comment, int score)
        {
            var userId = CurrentUserId;
            var game = _gamesRepository.GetById(gameId);

            if (string.IsNullOrWhiteSpace(userId) || game == null)
            {
                _logger.LogWarning("Home create entry rejected. UserId={UserId}, GameId={GameId}", userId, gameId);
                TempData["Error"] = "User or game was not found!";
                return RedirectToAction("Index");
            }

            // Kreiraj GameEntry
            var gameEntry = new GameEntry
            {
                GameId = game.Id,
                UserId = userId,
                Status = status,
                CreatedAt = DateTime.Now,
                HoursPlayed = hoursPlayed,
                ReviewScore = score,
                ReviewComment = string.IsNullOrWhiteSpace(comment) ? null : comment
            };
            _gameEntriesRepository.Add(gameEntry);

            _logger.LogInformation(
                "Game entry created from home. EntryId={EntryId}, UserId={UserId}, GameId={GameId}",
                gameEntry.Id,
                userId,
                game.Id);

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
