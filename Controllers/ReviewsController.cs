using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ReviewsRepository _reviewsRepository;

        public ReviewsController(ReviewsRepository reviewsRepository)
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
