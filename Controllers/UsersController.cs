using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;

namespace VideoGameTracker.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly UsersRepository _usersRepository;

        public UsersController(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var users = _usersRepository.GetAll();
            return View(users);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var user = _usersRepository.GetById(id);
            if (user == null)
                return NotFound();
            return View(user);
        }
    }
}
