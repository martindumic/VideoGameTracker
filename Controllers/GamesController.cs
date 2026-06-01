using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Route("games")]
    public class GamesController : Controller
    {
        private readonly GamesRepository _gamesRepository;
        private readonly DevelopersRepository _developersRepository;
        private readonly GenresRepository _genresRepository;
        private readonly PlatformsRepository _platformsRepository;
        private readonly GameEntriesRepository _gameEntriesRepository;
        private readonly GameEntryScreenshotsRepository _screenshotsRepository;
        private readonly ILogger<GamesController> _logger;

        public GamesController(
            GamesRepository gamesRepository,
            DevelopersRepository developersRepository,
            GenresRepository genresRepository,
            PlatformsRepository platformsRepository,
            GameEntriesRepository gameEntriesRepository,
            GameEntryScreenshotsRepository screenshotsRepository,
            ILogger<GamesController> logger)
        {
            _gamesRepository = gamesRepository;
            _developersRepository = developersRepository;
            _genresRepository = genresRepository;
            _platformsRepository = platformsRepository;
            _gameEntriesRepository = gameEntriesRepository;
            _screenshotsRepository = screenshotsRepository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var games = _gamesRepository.GetAll();
            return View(games);
        }

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _gamesRepository.GetAll()
                : _gamesRepository.Search(term);

            return PartialView("_GameTable", results);
        }

        [HttpGet("search-developers")]
        public IActionResult SearchDevelopers(string? term)
        {
            var results = _developersRepository.Search(term)
                .OrderBy(d => d.Name)
                .Take(12)
                .Select(d => new { id = d.Id, text = d.Name ?? string.Empty })
                .ToList();

            return Json(results);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = BuildFormViewModel(null);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GameFormViewModel model)
        {
            PrepareFormOptions(model);

            if (!ModelState.IsValid)
            {
                PopulateSelectedDeveloperName(model);
                return View(model);
            }

            var game = BuildGameEntity(model, 0);

            try
            {
                _gamesRepository.Create(game);
                _logger.LogInformation("Game created. GameId={GameId}, Title={Title}", game.Id, game.Title);
                TempData["Success"] = "Game created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create game. Title={Title}", game.Title);
                TempData["Error"] = "Unable to create game.";
                PopulateSelectedDeveloperName(model);
                return View(model);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var game = _gamesRepository.GetById(id);
            if (game == null)
                return NotFound();
            return View(game);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/screenshots")]
        public IActionResult Screenshots(int id, string? mode)
        {
            var game = _gamesRepository.GetById(id);
            if (game == null)
            {
                return NotFound();
            }

            var model = new GameEntryScreenshotListViewModel
            {
                GameEntryId = 0,
                CanManage = false,
                UseSlideshow = string.Equals(mode, "slideshow", StringComparison.OrdinalIgnoreCase),
                Screenshots = _screenshotsRepository.GetByGameId(id)
            };

            return PartialView("~/Views/GameEntries/_GameEntryScreenshots.cshtml", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var game = _gamesRepository.GetById(id);
            if (game == null)
            {
                return NotFound();
            }

            var model = BuildFormViewModel(game);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GameFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            PrepareFormOptions(model);

            if (!ModelState.IsValid)
            {
                PopulateSelectedDeveloperName(model);
                return View(model);
            }

            var game = BuildGameEntity(model, id);
            if (_gamesRepository.Update(game))
            {
                _logger.LogInformation("Game updated. GameId={GameId}, Title={Title}", game.Id, game.Title);
                TempData["Success"] = "Game updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Game update failed. GameId={GameId}", game.Id);
            TempData["Error"] = "Unable to update game.";
            PopulateSelectedDeveloperName(model);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var game = _gamesRepository.GetById(id);
            if (game == null)
            {
                return NotFound();
            }

            if (HasGameEntries(id))
            {
                _logger.LogWarning("Delete blocked for game with entries. GameId={GameId}", id);
                ViewData["DeleteBlocked"] = true;
                ViewData["DeleteReason"] = "This game has game entries and cannot be deleted.";
            }

            return View(game);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HasGameEntries(id))
            {
                _logger.LogWarning("Delete rejected for game with entries. GameId={GameId}", id);
                TempData["Error"] = "This game has game entries and cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (_gamesRepository.Delete(id))
            {
                _logger.LogInformation("Game deleted. GameId={GameId}", id);
                TempData["Success"] = "Game deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Game delete failed. GameId={GameId}", id);
            TempData["Error"] = "Unable to delete game.";
            return RedirectToAction(nameof(Index));
        }

        private GameFormViewModel BuildFormViewModel(Game? game)
        {
            var model = new GameFormViewModel
            {
                Id = game?.Id,
                Title = game?.Title,
                ReleaseYear = game?.ReleaseYear ?? DateTime.Now.Year,
                Description = game?.Description,
                DeveloperId = game?.DeveloperId,
                DeveloperName = game?.Developer?.Name,
                AverageRating = game?.AverageRating ?? 0,
                SelectedGenreIds = game?.Genres.Select(g => g.Id).ToList() ?? new List<int>(),
                SelectedPlatformIds = game?.Platforms.Select(p => p.Id).ToList() ?? new List<int>()
            };

            PrepareFormOptions(model);
            return model;
        }

        private void PrepareFormOptions(GameFormViewModel model)
        {
            model.GenreOptions = _genresRepository.GetAll()
                .OrderBy(g => g.Name)
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name ?? $"Genre {g.Id}",
                    Selected = model.SelectedGenreIds.Contains(g.Id)
                })
                .ToList();

            model.PlatformOptions = _platformsRepository.GetAll()
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name ?? $"Platform {p.Id}",
                    Selected = model.SelectedPlatformIds.Contains(p.Id)
                })
                .ToList();
        }

        private void PopulateSelectedDeveloperName(GameFormViewModel model)
        {
            if (!model.DeveloperId.HasValue)
            {
                model.DeveloperName = null;
                return;
            }

            var developer = _developersRepository.GetById(model.DeveloperId.Value);
            model.DeveloperName = developer?.Name;
        }

        private Game BuildGameEntity(GameFormViewModel model, int id)
        {
            var game = new Game
            {
                Id = id,
                Title = model.Title?.Trim(),
                ReleaseYear = model.ReleaseYear,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                DeveloperId = model.DeveloperId!.Value,
                AverageRating = model.AverageRating
            };

            game.Genres = model.SelectedGenreIds
                .Select(genreId => new Genre { Id = genreId })
                .ToList();

            game.Platforms = model.SelectedPlatformIds
                .Select(platformId => new Platform { Id = platformId })
                .ToList();

            return game;
        }

        private bool HasGameEntries(int gameId)
        {
            return _gameEntriesRepository.GetAll().Any(entry => entry.GameId == gameId);
        }
    }
}
