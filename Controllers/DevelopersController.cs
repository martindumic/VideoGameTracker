using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    [Route("developers")]
    public class DevelopersController : Controller
    {
        private readonly DevelopersRepository _developersRepository;

        public DevelopersController(DevelopersRepository developersRepository)
        {
            _developersRepository = developersRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var developers = _developersRepository.GetAll();
            return View(developers);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var developer = _developersRepository.GetById(id);
            if (developer == null)
                return NotFound();
            return View(developer);
        }
    }
}
