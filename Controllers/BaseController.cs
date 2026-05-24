using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers;

public abstract class BaseController : Controller
{
    protected BaseController(UserManager<AppUser> userManager)
    {
        UserManager = userManager;
    }

    protected UserManager<AppUser> UserManager { get; }

    protected string? CurrentUserId => UserManager.GetUserId(User);

    protected bool IsAdmin => User.IsInRole("Admin");
}
