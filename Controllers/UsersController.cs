using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly VideoGameTrackerDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(VideoGameTrackerDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var users = _dbContext.Users
                .Include(u => u.GameEntries)
                    .ThenInclude(e => e.Game)
                .ToList();

            return View(users);
        }

        [HttpGet("search")]
        public IActionResult Search(string? term)
        {
            var usersQuery = _dbContext.Users
                .Include(u => u.GameEntries)
                    .ThenInclude(e => e.Game)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var likeTerm = $"%{term.Trim()}%";
                usersQuery = usersQuery.Where(u =>
                    EF.Functions.Like(u.UserName ?? string.Empty, likeTerm) ||
                    EF.Functions.Like(u.Email ?? string.Empty, likeTerm));
            }

            return PartialView("_UserTable", usersQuery.ToList());
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = BuildFormViewModel(null, true);
            return View(model);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
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

            var user = new AppUser
            {
                UserName = model.Username?.Trim(),
                Email = model.Email?.Trim(),
                OIB = model.OIB?.Trim() ?? string.Empty,
                JMBG = model.JMBG?.Trim() ?? string.Empty,
                RegisteredAt = registeredAt
            };

            var result = await _userManager.CreateAsync(user, model.Password!.Trim());
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Player");
                TempData["Success"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            TempData["Error"] = "Unable to create user.";
            return View(model);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var user = _dbContext.Users
                .Include(u => u.GameEntries)
                    .ThenInclude(e => e.Game)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet("{id}/edit")]
        public IActionResult Edit(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = BuildFormViewModel(user, false);
            return View(model);
        }

        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserFormViewModel model)
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

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.Username?.Trim();
            user.Email = model.Email?.Trim();
            user.OIB = model.OIB?.Trim() ?? string.Empty;
            user.JMBG = model.JMBG?.Trim() ?? string.Empty;
            user.RegisteredAt = registeredAt;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                TempData["Error"] = "Unable to update user.";
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password.Trim());
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    TempData["Error"] = "Unable to update user password.";
                    return View(model);
                }
            }

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id}/delete")]
        public IActionResult Delete(string id)
        {
            var user = _dbContext.Users
                .Include(u => u.GameEntries)
                .FirstOrDefault(u => u.Id == id);

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

        [HttpPost("{id}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = _dbContext.Users
                .Include(u => u.GameEntries)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.GameEntries.Any())
            {
                TempData["Error"] = "This user has game entries and cannot be deleted.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Unable to delete user.";
            return RedirectToAction(nameof(Index));
        }

        private static UserFormViewModel BuildFormViewModel(AppUser? user, bool requirePassword)
        {
            return new UserFormViewModel
            {
                Id = user?.Id,
                Username = user?.UserName,
                Email = user?.Email,
                OIB = user?.OIB,
                JMBG = user?.JMBG,
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
