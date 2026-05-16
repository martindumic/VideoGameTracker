using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Route("game-entries")]
    public class GameEntriesController : Controller
    {
        private readonly GameEntriesRepository _gameEntriesRepository;
        private readonly GamesRepository _gamesRepository;
        private readonly UsersRepository _usersRepository;

        public GameEntriesController(
            GameEntriesRepository gameEntriesRepository,
            GamesRepository gamesRepository,
            UsersRepository usersRepository)
        {
            _gameEntriesRepository = gameEntriesRepository;
            _gamesRepository = gamesRepository;
            _usersRepository = usersRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var gameEntries = _gameEntriesRepository.GetAll();
            var model = BuildIndexViewModel(gameEntries);
            return View(model);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
                return NotFound();
            return View(gameEntry);
        }

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _gameEntriesRepository.GetAll()
                : _gameEntriesRepository.Search(term);

            return PartialView("_GameEntryTable", results);
        }

        [HttpGet("search-games")]
        public IActionResult SearchGames(string? term)
        {
            var results = _gamesRepository.Search(term)
                .OrderBy(g => g.Title)
                .Take(12)
                .Select(g => new { id = g.Id, text = g.Title ?? string.Empty })
                .ToList();

            return Json(results);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = BuildFormViewModel(null, "Create");
            return View(model);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GameEntryFormViewModel model)
        {
            model.CanSelectUser = true;
            PrepareFormOptions(model);

            if (!TryParseReviewTimestamp(model.ReviewTimestamp, out var reviewTimestamp))
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.ReviewTimestamp), "Review date and time format is invalid.");
            }

            if (!model.UserId.HasValue)
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.UserId), "User is required.");
            }

            if (!ModelState.IsValid)
            {
                PopulateSelectedGameTitle(model);
                if (string.Equals(model.FormContext, "Index", StringComparison.OrdinalIgnoreCase))
                {
                    var indexModel = BuildIndexViewModel(_gameEntriesRepository.GetAll());
                    indexModel.Form = model;
                    return View("Index", indexModel);
                }

                return View(model);
            }

            var gameEntry = new GameEntry
            {
                GameId = model.GameId!.Value,
                UserId = model.UserId!.Value,
                Status = model.Status!.Value,
                HoursPlayed = model.HoursPlayed,
                ReviewScore = model.ReviewScore,
                ReviewComment = string.IsNullOrWhiteSpace(model.ReviewComment) ? null : model.ReviewComment.Trim(),
                CreatedAt = reviewTimestamp
            };

            try
            {
                _gameEntriesRepository.Create(gameEntry);
                TempData["Success"] = "Game entry created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unable to create game entry.";
                PopulateSelectedGameTitle(model);
                if (string.Equals(model.FormContext, "Index", StringComparison.OrdinalIgnoreCase))
                {
                    var indexModel = BuildIndexViewModel(_gameEntriesRepository.GetAll());
                    indexModel.Form = model;
                    return View("Index", indexModel);
                }

                return View(model);
            }
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            var model = BuildFormViewModel(gameEntry, "Edit");
            return View(model);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GameEntryFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            model.CanSelectUser = true;
            PrepareFormOptions(model);

            if (!TryParseReviewTimestamp(model.ReviewTimestamp, out var reviewTimestamp))
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.ReviewTimestamp), "Review date and time format is invalid.");
            }

            if (!model.UserId.HasValue)
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.UserId), "User is required.");
            }

            if (!ModelState.IsValid)
            {
                PopulateSelectedGameTitle(model);
                return View(model);
            }

            var gameEntry = new GameEntry
            {
                Id = id,
                GameId = model.GameId!.Value,
                UserId = model.UserId!.Value,
                Status = model.Status!.Value,
                HoursPlayed = model.HoursPlayed,
                ReviewScore = model.ReviewScore,
                ReviewComment = string.IsNullOrWhiteSpace(model.ReviewComment) ? null : model.ReviewComment.Trim(),
                CreatedAt = reviewTimestamp
            };

            if (_gameEntriesRepository.Update(gameEntry))
            {
                TempData["Success"] = "Game entry updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Unable to update game entry.";
            PopulateSelectedGameTitle(model);
            return View(model);
        }

        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            return View(gameEntry);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_gameEntriesRepository.Delete(id))
            {
                TempData["Success"] = "Game entry deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Unable to delete game entry.";
            return RedirectToAction(nameof(Index));
        }

        private GameEntryIndexViewModel BuildIndexViewModel(List<GameEntry> entries)
        {
            var model = new GameEntryIndexViewModel
            {
                Entries = entries,
                Form = BuildFormViewModel(null, "Index")
            };

            return model;
        }

        private GameEntryFormViewModel BuildFormViewModel(GameEntry? entry, string formContext)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var sessionUsername = HttpContext.Session.GetString("Username");
            var canSelectUser = true;

            var model = new GameEntryFormViewModel
            {
            FormContext = formContext,
                Id = entry?.Id,
                GameId = entry?.GameId,
                GameTitle = entry?.Game?.Title,
                UserId = entry?.UserId ?? sessionUserId,
                Username = entry?.User?.Username ?? sessionUsername,
                Status = entry?.Status,
                HoursPlayed = entry?.HoursPlayed ?? 0,
                ReviewScore = entry?.ReviewScore,
                ReviewComment = entry?.ReviewComment,
                ReviewTimestamp = (entry?.CreatedAt ?? DateTime.Now)
                    .ToString("g", CultureInfo.CurrentCulture),
                CanSelectUser = canSelectUser
            };

            PrepareFormOptions(model);
            return model;
        }

        private void PrepareFormOptions(GameEntryFormViewModel model)
        {
            model.StatusOptions = Enum.GetValues<GameStatus>()
                .Select(status => new SelectListItem
                {
                    Value = status.ToString(),
                    Text = status.ToString(),
                    Selected = model.Status == status
                })
                .ToList();

            if (model.CanSelectUser)
            {
                model.UserOptions = _usersRepository.GetAll()
                    .OrderBy(u => u.Username)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Username ?? $"User {u.Id}",
                        Selected = model.UserId == u.Id
                    })
                    .ToList();
            }
        }

        private void PopulateSelectedGameTitle(GameEntryFormViewModel model)
        {
            if (!model.GameId.HasValue)
            {
                model.GameTitle = null;
                return;
            }

            var game = _gamesRepository.GetById(model.GameId.Value);
            model.GameTitle = game?.Title;
        }

        private static bool TryParseReviewTimestamp(string? value, out DateTime parsed)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                parsed = default;
                return false;
            }

            var styles = DateTimeStyles.AllowWhiteSpaces;
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, styles, out parsed))
            {
                return true;
            }

            if (DateTime.TryParse(value, new CultureInfo("hr-HR"), styles, out parsed))
            {
                return true;
            }

            if (DateTime.TryParse(value, new CultureInfo("en-US"), styles, out parsed))
            {
                return true;
            }

            return DateTime.TryParse(value, CultureInfo.InvariantCulture, styles, out parsed);
        }
    }
}
