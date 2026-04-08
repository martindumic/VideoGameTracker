using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ReviewsMockRepository _reviewsRepository;

        public ReviewsController(ReviewsMockRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        public IActionResult Index()
        {
            var reviews = _reviewsRepository.GetAll();
            return View(reviews);
        }

        public IActionResult Details(int id)
        {
            var review = _reviewsRepository.GetById(id);
            if (review == null)
                return NotFound();
            return View(review);
        }
    }
}
