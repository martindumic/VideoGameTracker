using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsersRepository _usersRepository;

        public LoginController(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Ako je korisnik već prijavljen, preusmjeri na Home
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required!");
                return View();
            }

            var user = _usersRepository.GetAll()
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is incorrect!");
                return View();
            }

            // Spremi korisnika u session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username ?? "");
            HttpContext.Session.SetString("Email", user.Email ?? "");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
