using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Route("genres")]
    public class GenresController : Controller
    {
        private readonly GenresRepository _genresRepository;
        private readonly ILogger<GenresController> _logger;

        public GenresController(GenresRepository genresRepository, ILogger<GenresController> logger)
        {
            _genresRepository = genresRepository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var genres = _genresRepository.GetAll();
            return View(genres);
        }

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _genresRepository.GetAll()
                : _genresRepository.Search(term);

            return PartialView("_GenreTable", results);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new GenreFormViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GenreFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var genre = new Genre
            {
                Name = model.Name?.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            };

            try
            {
                _genresRepository.Create(genre);
                _logger.LogInformation("Genre created. GenreId={GenreId}, Name={Name}", genre.Id, genre.Name);
                TempData["Success"] = "Genre created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create genre. Name={Name}", genre.Name);
                TempData["Error"] = "Unable to create genre.";
                return View(model);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null)
                return NotFound();
            return View(genre);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }

            var model = new GenreFormViewModel
            {
                Id = genre.Id,
                Name = genre.Name,
                Description = genre.Description
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GenreFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var genre = new Genre
            {
                Id = id,
                Name = model.Name?.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            };

            if (_genresRepository.Update(genre))
            {
                _logger.LogInformation("Genre updated. GenreId={GenreId}, Name={Name}", genre.Id, genre.Name);
                TempData["Success"] = "Genre updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Genre update failed. GenreId={GenreId}", genre.Id);
            TempData["Error"] = "Unable to update genre.";
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }

            if (genre.Games.Any())
            {
                _logger.LogWarning("Delete blocked for genre with games. GenreId={GenreId}", id);
                ViewData["DeleteBlocked"] = true;
                ViewData["DeleteReason"] = "This genre is assigned to games and cannot be deleted.";
            }

            return View(genre);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }

            if (genre.Games.Any())
            {
                _logger.LogWarning("Delete rejected for genre with games. GenreId={GenreId}", id);
                TempData["Error"] = "This genre is assigned to games and cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (_genresRepository.Delete(id))
            {
                _logger.LogInformation("Genre deleted. GenreId={GenreId}", id);
                TempData["Success"] = "Genre deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Genre delete failed. GenreId={GenreId}", id);
            TempData["Error"] = "Unable to delete genre.";
            return RedirectToAction(nameof(Index));
        }
    }
}
