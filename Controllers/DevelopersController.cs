using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Route("developers")]
    public class DevelopersController : Controller
    {
        private readonly DevelopersRepository _developersRepository;
        private readonly ILogger<DevelopersController> _logger;

        public DevelopersController(DevelopersRepository developersRepository, ILogger<DevelopersController> logger)
        {
            _developersRepository = developersRepository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var developers = _developersRepository.GetAll();
            return View(developers);
        }

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _developersRepository.GetAll()
                : _developersRepository.Search(term);

            return PartialView("_DeveloperTable", results);
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
        public IActionResult Create(DeveloperFormViewModel model)
        {
            if (!TryParseDate(model.Founded, out var founded))
            {
                ModelState.AddModelError(nameof(DeveloperFormViewModel.Founded), "Founded date format is invalid.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var developer = new Developer
            {
                Name = model.Name?.Trim(),
                Country = model.Country?.Trim(),
                Founded = founded,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            };

            try
            {
                _developersRepository.Create(developer);
                _logger.LogInformation("Developer created. DeveloperId={DeveloperId}, Name={Name}", developer.Id, developer.Name);
                TempData["Success"] = "Developer created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create developer. Name={Name}", developer.Name);
                TempData["Error"] = "Unable to create developer.";
                return View(model);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var developer = _developersRepository.GetById(id);
            if (developer == null)
                return NotFound();
            return View(developer);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var developer = _developersRepository.GetById(id);
            if (developer == null)
            {
                return NotFound();
            }

            var model = BuildFormViewModel(developer);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DeveloperFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!TryParseDate(model.Founded, out var founded))
            {
                ModelState.AddModelError(nameof(DeveloperFormViewModel.Founded), "Founded date format is invalid.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var developer = new Developer
            {
                Id = id,
                Name = model.Name?.Trim(),
                Country = model.Country?.Trim(),
                Founded = founded,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            };

            if (_developersRepository.Update(developer))
            {
                _logger.LogInformation("Developer updated. DeveloperId={DeveloperId}, Name={Name}", developer.Id, developer.Name);
                TempData["Success"] = "Developer updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Developer update failed. DeveloperId={DeveloperId}", developer.Id);
            TempData["Error"] = "Unable to update developer.";
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var developer = _developersRepository.GetById(id);
            if (developer == null)
            {
                return NotFound();
            }

            if (developer.Games.Any())
            {
                _logger.LogWarning("Delete blocked for developer with games. DeveloperId={DeveloperId}", id);
                ViewData["DeleteBlocked"] = true;
                ViewData["DeleteReason"] = "This developer has games and cannot be deleted.";
            }

            return View(developer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var developer = _developersRepository.GetById(id);
            if (developer == null)
            {
                return NotFound();
            }

            if (developer.Games.Any())
            {
                _logger.LogWarning("Delete rejected for developer with games. DeveloperId={DeveloperId}", id);
                TempData["Error"] = "This developer has games and cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (_developersRepository.Delete(id))
            {
                _logger.LogInformation("Developer deleted. DeveloperId={DeveloperId}", id);
                TempData["Success"] = "Developer deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Developer delete failed. DeveloperId={DeveloperId}", id);
            TempData["Error"] = "Unable to delete developer.";
            return RedirectToAction(nameof(Index));
        }

        private DeveloperFormViewModel BuildFormViewModel(Developer? developer)
        {
            return new DeveloperFormViewModel
            {
                Id = developer?.Id,
                Name = developer?.Name,
                Country = developer?.Country,
                Founded = (developer?.Founded ?? DateTime.Now)
                    .ToString("g", CultureInfo.CurrentCulture),
                Description = developer?.Description
            };
        }

        private static bool TryParseDate(string? value, out DateTime parsed)
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
