using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    [Route("reviews")]
    public class ReviewsController : Controller
    {
        private readonly ReviewsRepository _reviewsRepository;

        public ReviewsController(ReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var reviews = _reviewsRepository.GetAll();
            return View(reviews);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var review = _reviewsRepository.GetById(id);
            if (review == null)
                return NotFound();
            return View(review);
        }
    }
}
