using System.Globalization;
using System.IO;
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
        private readonly GameEntryScreenshotsRepository _screenshotsRepository;
        private readonly ILogger<GameEntriesController> _logger;

        public GameEntriesController(
            GameEntriesRepository gameEntriesRepository,
            GamesRepository gamesRepository,
            GameEntryScreenshotsRepository screenshotsRepository,
            UserManager<AppUser> userManager,
            ILogger<GameEntriesController> logger)
            : base(userManager)
        {
            _gameEntriesRepository = gameEntriesRepository;
            _gamesRepository = gamesRepository;
            _screenshotsRepository = screenshotsRepository;
            _logger = logger;
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
                    _logger.LogWarning("Create game entry blocked. Missing user id.");
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
                _logger.LogInformation(
                    "Game entry created. EntryId={EntryId}, UserId={UserId}, GameId={GameId}",
                    gameEntry.Id,
                    gameEntry.UserId,
                    gameEntry.GameId);
                TempData["Success"] = "Game entry created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create game entry. UserId={UserId}, GameId={GameId}", gameEntry.UserId, gameEntry.GameId);
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
                _logger.LogWarning(
                    "Edit forbidden for game entry. EntryId={EntryId}, UserId={UserId}",
                    gameEntry.Id,
                    CurrentUserId);
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
                _logger.LogWarning(
                    "Edit forbidden for game entry. EntryId={EntryId}, UserId={UserId}",
                    existing.Id,
                    CurrentUserId);
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
                _logger.LogInformation(
                    "Game entry updated. EntryId={EntryId}, UserId={UserId}, GameId={GameId}",
                    gameEntry.Id,
                    gameEntry.UserId,
                    gameEntry.GameId);
                TempData["Success"] = "Game entry updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Game entry update failed. EntryId={EntryId}", gameEntry.Id);
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
                _logger.LogWarning(
                    "Delete forbidden for game entry. EntryId={EntryId}, UserId={UserId}",
                    gameEntry.Id,
                    CurrentUserId);
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
                _logger.LogWarning(
                    "Delete forbidden for game entry. EntryId={EntryId}, UserId={UserId}",
                    entry.Id,
                    CurrentUserId);
                TempData["Error"] = "You do not have permission to delete this game entry.";
                return RedirectToAction(nameof(Index));
            }

            if (_gameEntriesRepository.Delete(id))
            {
                _logger.LogInformation(
                    "Game entry deleted. EntryId={EntryId}, UserId={UserId}",
                    id,
                    entry.UserId);
                TempData["Success"] = "Game entry deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Game entry delete failed. EntryId={EntryId}", id);
            TempData["Error"] = "Unable to delete game entry.";
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/screenshots")]
        public IActionResult Screenshots(int id, [FromQuery] bool allowManage = true, [FromQuery] string? mode = null)
        {
            var entry = _gameEntriesRepository.GetById(id);
            if (entry == null)
            {
                return NotFound();
            }

            var canManage = allowManage && CanManageScreenshots(entry);
            var model = new GameEntryScreenshotListViewModel
            {
                GameEntryId = id,
                CanManage = canManage,
                UseSlideshow = string.Equals(mode, "slideshow", StringComparison.OrdinalIgnoreCase),
                Screenshots = _screenshotsRepository.GetByGameEntryId(id)
            };

            return PartialView("_GameEntryScreenshots", model);
        }

        [Authorize]
        [HttpPost("{id:int}/screenshots")]
        [ValidateAntiForgeryToken]
        public IActionResult UploadScreenshot(int id, IFormFile file)
        {
            var entry = _gameEntriesRepository.GetById(id);
            if (entry == null)
            {
                return NotFound();
            }

            if (!CanManageScreenshots(entry))
            {
                _logger.LogWarning(
                    "Screenshot upload forbidden. EntryId={EntryId}, UserId={UserId}",
                    entry.Id,
                    CurrentUserId);
                return Forbid();
            }

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Screenshot upload rejected. Empty file. EntryId={EntryId}", id);
                return BadRequest("File is required.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning("Screenshot upload rejected. File too large. EntryId={EntryId}", id);
                return BadRequest("File is too large.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Screenshot upload rejected. Invalid extension. EntryId={EntryId}", id);
                return BadRequest("Invalid file type.");
            }

            var allowedContentTypes = new HashSet<string>
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                _logger.LogWarning("Screenshot upload rejected. Invalid content type. EntryId={EntryId}", id);
                return BadRequest("Invalid content type.");
            }

            var uploadsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "gameentries",
                id.ToString());

            Directory.CreateDirectory(uploadsPath);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(uploadsPath, storedFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var screenshot = new GameEntryScreenshot
            {
                GameEntryId = id,
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = storedFileName,
                FilePath = $"/uploads/gameentries/{id}/{storedFileName}",
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = CurrentUserId
            };

            _screenshotsRepository.Create(screenshot);
            _logger.LogInformation(
                "Screenshot uploaded. EntryId={EntryId}, ScreenshotId={ScreenshotId}, UserId={UserId}",
                id,
                screenshot.Id,
                CurrentUserId);
            return Ok(new { success = true });
        }

        [Authorize]
        [HttpPost("{id:int}/screenshots/{screenshotId:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteScreenshot(int id, int screenshotId)
        {
            var entry = _gameEntriesRepository.GetById(id);
            if (entry == null)
            {
                return NotFound();
            }

            if (!CanManageScreenshots(entry))
            {
                _logger.LogWarning(
                    "Screenshot delete forbidden. EntryId={EntryId}, UserId={UserId}",
                    entry.Id,
                    CurrentUserId);
                return Forbid();
            }

            var screenshot = _screenshotsRepository.GetById(screenshotId);
            if (screenshot == null || screenshot.GameEntryId != id)
            {
                _logger.LogWarning("Screenshot delete failed. EntryId={EntryId}, ScreenshotId={ScreenshotId}", id, screenshotId);
                return NotFound();
            }

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                screenshot.FilePath.TrimStart('/'));

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _screenshotsRepository.Delete(screenshot);
            _logger.LogInformation(
                "Screenshot deleted. EntryId={EntryId}, ScreenshotId={ScreenshotId}, UserId={UserId}",
                id,
                screenshot.Id,
                CurrentUserId);
            return Ok(new { success = true });
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

        private bool CanManageScreenshots(GameEntry entry)
        {
            if (IsAdmin)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(CurrentUserId) && entry.UserId == CurrentUserId;
        }
    }
}
