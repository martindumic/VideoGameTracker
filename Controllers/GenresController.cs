using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    [Route("genres")]
    public class GenresController : Controller
    {
        private readonly GenresRepository _genresRepository;

        public GenresController(GenresRepository genresRepository)
        {
            _genresRepository = genresRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var genres = _genresRepository.GetAll();
            return View(genres);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var genre = _genresRepository.GetById(id);
            if (genre == null)
                return NotFound();
            return View(genre);
        }
    }
}
