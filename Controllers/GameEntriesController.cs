using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Route("game-entries")]
    public class GameEntriesController : BaseController
    {
        private readonly GameEntriesRepository _gameEntriesRepository;
        private readonly GamesRepository _gamesRepository;

        public GameEntriesController(
            GameEntriesRepository gameEntriesRepository,
            GamesRepository gamesRepository,
            UserManager<AppUser> userManager)
            : base(userManager)
        {
            _gameEntriesRepository = gameEntriesRepository;
            _gamesRepository = gamesRepository;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public IActionResult Index()
        {
            var gameEntries = _gameEntriesRepository.GetAll();
            var model = BuildIndexViewModel(gameEntries);
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
                return NotFound();
            return View(gameEntry);
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _gameEntriesRepository.GetAll()
                : _gameEntriesRepository.Search(term);

            return PartialView("_GameEntryTable", results);
        }

        [AllowAnonymous]
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

        [Authorize]
        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = BuildFormViewModel(null, "Create");
            return View(model);
        }

        [Authorize]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GameEntryFormViewModel model)
        {
            model.CanSelectUser = IsAdmin;
            PrepareFormOptions(model);

            if (!IsAdmin)
            {
                if (string.IsNullOrWhiteSpace(CurrentUserId))
                {
                    return Challenge();
                }

                model.UserId = CurrentUserId;
            }

            if (!TryParseReviewTimestamp(model.ReviewTimestamp, out var reviewTimestamp))
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.ReviewTimestamp), "Review date and time format is invalid.");
            }

            if (string.IsNullOrWhiteSpace(model.UserId))
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
                UserId = model.UserId!,
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

        [Authorize]
        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            if (!IsAdmin && gameEntry.UserId != CurrentUserId)
            {
                TempData["Error"] = "You do not have permission to edit this game entry.";
                return RedirectToAction(nameof(Index));
            }

            var model = BuildFormViewModel(gameEntry, "Edit");
            return View(model);
        }

        [Authorize]
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GameEntryFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            model.CanSelectUser = IsAdmin;
            PrepareFormOptions(model);

            if (!TryParseReviewTimestamp(model.ReviewTimestamp, out var reviewTimestamp))
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.ReviewTimestamp), "Review date and time format is invalid.");
            }

            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.UserId), "User is required.");
            }

            if (!ModelState.IsValid)
            {
                PopulateSelectedGameTitle(model);
                return View(model);
            }

            var existing = _gameEntriesRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (!IsAdmin && existing.UserId != CurrentUserId)
            {
                TempData["Error"] = "You do not have permission to edit this game entry.";
                return RedirectToAction(nameof(Index));
            }

            var userId = IsAdmin ? model.UserId : existing.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError(nameof(GameEntryFormViewModel.UserId), "User is required.");
                PopulateSelectedGameTitle(model);
                return View(model);
            }

            var gameEntry = new GameEntry
            {
                Id = id,
                GameId = model.GameId!.Value,
                UserId = userId,
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

        [Authorize]
        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var gameEntry = _gameEntriesRepository.GetById(id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            if (!IsAdmin && gameEntry.UserId != CurrentUserId)
            {
                TempData["Error"] = "You do not have permission to delete this game entry.";
                return RedirectToAction(nameof(Index));
            }

            return View(gameEntry);
        }

        [Authorize]
        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var entry = _gameEntriesRepository.GetById(id);
            if (entry == null)
            {
                return NotFound();
            }

            if (!IsAdmin && entry.UserId != CurrentUserId)
            {
                TempData["Error"] = "You do not have permission to delete this game entry.";
                return RedirectToAction(nameof(Index));
            }

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
            var canSelectUser = IsAdmin;
            var selectedUserId = entry?.UserId ?? (canSelectUser ? null : CurrentUserId);
            var selectedUsername = entry?.User?.UserName ?? UserManager.GetUserName(User);

            var model = new GameEntryFormViewModel
            {
                FormContext = formContext,
                Id = entry?.Id,
                GameId = entry?.GameId,
                GameTitle = entry?.Game?.Title,
                UserId = selectedUserId,
                Username = selectedUsername,
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
                model.UserOptions = UserManager.Users
                    .OrderBy(u => u.UserName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.UserName ?? u.Email ?? u.Id,
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
