using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class DevelopersController : Controller
    {
        private readonly DevelopersRepository _developersRepository;

        public DevelopersController(DevelopersRepository developersRepository)
        {
            _developersRepository = developersRepository;
        }

        public IActionResult Index()
        {
            var developers = _developersRepository.GetAll();
            return View(developers);
        }

        public IActionResult Details(int id)
        {
            var developer = _developersRepository.GetById(id);
            if (developer == null)
                return NotFound();
            return View(developer);
        }
    }
}
