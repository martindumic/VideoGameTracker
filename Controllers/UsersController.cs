using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

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

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var results = string.IsNullOrWhiteSpace(term)
                ? _usersRepository.GetAll()
                : _usersRepository.Search(term);

            return PartialView("_UserTable", results);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = BuildFormViewModel(null, true);
            return View(model);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserFormViewModel model)
        {
            model.RequirePassword = true;

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(UserFormViewModel.Password), "Password is required.");
            }

            if (!TryParseDate(model.RegisteredAt, out var registeredAt))
            {
                ModelState.AddModelError(nameof(UserFormViewModel.RegisteredAt), "Registered date format is invalid.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Username = model.Username?.Trim(),
                Email = model.Email?.Trim(),
                Password = model.Password?.Trim(),
                RegisteredAt = registeredAt
            };

            try
            {
                _usersRepository.Create(user);
                TempData["Success"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unable to create user.";
                return View(model);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var user = _usersRepository.GetById(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var user = _usersRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = BuildFormViewModel(user, false);
            return View(model);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            model.RequirePassword = false;

            if (!TryParseDate(model.RegisteredAt, out var registeredAt))
            {
                ModelState.AddModelError(nameof(UserFormViewModel.RegisteredAt), "Registered date format is invalid.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = _usersRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            var password = string.IsNullOrWhiteSpace(model.Password)
                ? existing.Password
                : model.Password?.Trim();

            var user = new User
            {
                Id = id,
                Username = model.Username?.Trim(),
                Email = model.Email?.Trim(),
                Password = password,
                RegisteredAt = registeredAt
            };

            if (_usersRepository.Update(user))
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Unable to update user.";
            return View(model);
        }

        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var user = _usersRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.GameEntries.Any())
            {
                ViewData["DeleteBlocked"] = true;
                ViewData["DeleteReason"] = "This user has game entries and cannot be deleted.";
            }

            return View(user);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _usersRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.GameEntries.Any())
            {
                TempData["Error"] = "This user has game entries and cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            if (_usersRepository.Delete(id))
            {
                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Unable to delete user.";
            return RedirectToAction(nameof(Index));
        }

        private UserFormViewModel BuildFormViewModel(User? user, bool requirePassword)
        {
            return new UserFormViewModel
            {
                Id = user?.Id,
                Username = user?.Username,
                Email = user?.Email,
                Password = string.Empty,
                RegisteredAt = (user?.RegisteredAt ?? DateTime.Now)
                    .ToString("g", CultureInfo.CurrentCulture),
                RequirePassword = requirePassword
            };
        }

        private static bool TryParseDate(string? value, out DateTime parsed)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                parsed = default;
                return false;
            }

            var styles = DateTimeStyles.AllowWhiteSpaces;
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, styles, out parsed))
            {
                return true;
            }

            if (DateTime.TryParse(value, new CultureInfo("hr-HR"), styles, out parsed))
            {
                return true;
            }

            if (DateTime.TryParse(value, new CultureInfo("en-US"), styles, out parsed))
            {
                return true;
            }

            return DateTime.TryParse(value, CultureInfo.InvariantCulture, styles, out parsed);
        }
    }
}
