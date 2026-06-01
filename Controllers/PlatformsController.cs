using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Route("platforms")]
    public class PlatformsController : Controller
    {
        private readonly PlatformsRepository _platformsRepository;
        private readonly ILogger<PlatformsController> _logger;

        public PlatformsController(PlatformsRepository platformsRepository, ILogger<PlatformsController> logger)
        {
            _platformsRepository = platformsRepository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var platforms = _platformsRepository.GetAll();
            return View(platforms);
        }

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _platformsRepository.GetAll()
                : _platformsRepository.Search(term);

            return PartialView("_PlatformTable", results);
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
        public IActionResult Create(PlatformFormViewModel model)
        {
            PrepareTypeOptions(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var platform = new Platform
            {
                Name = model.Name?.Trim(),
                Type = model.Type!.Value
            };

            try
            {
                _platformsRepository.Create(platform);
                _logger.LogInformation("Platform created. PlatformId={PlatformId}, Name={Name}, Type={Type}", platform.Id, platform.Name, platform.Type);
                TempData["Success"] = "Platform created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create platform. Name={Name}", platform.Name);
                TempData["Error"] = "Unable to create platform.";
                return View(model);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var platform = _platformsRepository.GetById(id);
            if (platform == null)
                return NotFound();
            return View(platform);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var platform = _platformsRepository.GetById(id);
            if (platform == null)
            {
                return NotFound();
            }

            var model = BuildFormViewModel(platform);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PlatformFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            PrepareTypeOptions(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var platform = new Platform
            {
                Id = id,
                Name = model.Name?.Trim(),
                Type = model.Type!.Value
            };

            if (_platformsRepository.Update(platform))
            {
                _logger.LogInformation("Platform updated. PlatformId={PlatformId}, Name={Name}, Type={Type}", platform.Id, platform.Name, platform.Type);
                TempData["Success"] = "Platform updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Platform update failed. PlatformId={PlatformId}", platform.Id);
            TempData["Error"] = "Unable to update platform.";
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var platform = _platformsRepository.GetById(id);
            if (platform == null)
            {
                return NotFound();
            }

            if (platform.Games.Any())
            {
                _logger.LogWarning("Delete blocked for platform with games. PlatformId={PlatformId}", id);
                ViewData["DeleteBlocked"] = true;
                ViewData["DeleteReason"] = "This platform is assigned to games and cannot be deleted.";
            }

            return View(platform);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var platform = _platformsRepository.GetById(id);
            if (platform == null)
            {
                return NotFound();
            }

            if (platform.Games.Any())
            {
                _logger.LogWarning("Delete rejected for platform with games. PlatformId={PlatformId}", id);
                TempData["Error"] = "This platform is assigned to games and cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (_platformsRepository.Delete(id))
            {
                _logger.LogInformation("Platform deleted. PlatformId={PlatformId}", id);
                TempData["Success"] = "Platform deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Platform delete failed. PlatformId={PlatformId}", id);
            TempData["Error"] = "Unable to delete platform.";
            return RedirectToAction(nameof(Index));
        }

        private PlatformFormViewModel BuildFormViewModel(Platform? platform)
        {
            var model = new PlatformFormViewModel
            {
                Id = platform?.Id,
                Name = platform?.Name,
                Type = platform?.Type
            };

            PrepareTypeOptions(model);
            return model;
        }

        private void PrepareTypeOptions(PlatformFormViewModel model)
        {
            model.TypeOptions = Enum.GetValues<PlatformType>()
                .Select(type => new SelectListItem
                {
                    Value = type.ToString(),
                    Text = type.ToString(),
                    Selected = model.Type == type
                })
                .ToList();
        }
    }
}
