using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class PlatformsController : Controller
    {
        private readonly PlatformsMockRepository _platformsRepository;

        public PlatformsController(PlatformsMockRepository platformsRepository)
        {
            _platformsRepository = platformsRepository;
        }

        public IActionResult Index()
        {
            var platforms = _platformsRepository.GetAll();
            return View(platforms);
        }

        public IActionResult Details(int id)
        {
            var platform = _platformsRepository.GetById(id);
            if (platform == null)
                return NotFound();
            return View(platform);
        }
    }
}
