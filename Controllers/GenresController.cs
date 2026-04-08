using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class GenresController : Controller
    {
        private readonly GenresMockRepository _genresRepository;

        public GenresController(GenresMockRepository genresRepository)
        {
            _genresRepository = genresRepository;
        }

        public IActionResult Index()
        {
            var genres = _genresRepository.GetAll();
            return View(genres);
        }

        public IActionResult Details(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null)
                return NotFound();
            return View(genre);
        }
    }
}
