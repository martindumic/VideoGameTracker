using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideoGameTracker.Models;

namespace VideoGameTracker.Areas.Identity.Pages.Account;

public class ExternalLoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ExternalLoginModel> _logger;

    public ExternalLoginModel(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        ILogger<ExternalLoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ProviderDisplayName { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$")]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression("^[0-9]*$")]
        public string JMBG { get; set; } = string.Empty;
    }

    public IActionResult OnGet()
    {
        return RedirectToPage("./Login");
    }

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"External provider error: {remoteError}");
            return Page();
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToPage("./Login");
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrWhiteSpace(email))
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: false, info.LoginProvider);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in addLoginResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        ReturnUrl = returnUrl;
        ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
        Input = new InputModel
        {
            Email = email ?? string.Empty
        };

        return Page();
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToPage("./Login");
        }

        if (!ModelState.IsValid)
        {
            ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
            ReturnUrl = returnUrl;
            return Page();
        }

        var existing = await _userManager.FindByEmailAsync(Input.Email);
        if (existing != null)
        {
            if (string.IsNullOrWhiteSpace(existing.OIB))
            {
                existing.OIB = Input.OIB.Trim();
            }

            if (string.IsNullOrWhiteSpace(existing.JMBG))
            {
                existing.JMBG = Input.JMBG.Trim();
            }

            var updateResult = await _userManager.UpdateAsync(existing);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
                ReturnUrl = returnUrl;
                return Page();
            }

            var linkResult = await _userManager.AddLoginAsync(existing, info);
            if (!linkResult.Succeeded)
            {
                foreach (var error in linkResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
                ReturnUrl = returnUrl;
                return Page();
            }

            await _signInManager.SignInAsync(existing, isPersistent: false, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }

        var user = new AppUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            OIB = Input.OIB.Trim(),
            JMBG = Input.JMBG.Trim(),
            RegisteredAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
            ReturnUrl = returnUrl;
            return Page();
        }

        await _userManager.AddToRoleAsync(user, "Player");

        var loginResult = await _userManager.AddLoginAsync(user, info);
        if (!loginResult.Succeeded)
        {
            foreach (var error in loginResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ProviderDisplayName = info.ProviderDisplayName ?? string.Empty;
            ReturnUrl = returnUrl;
            return Page();
        }

        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
        return LocalRedirect(returnUrl);
    }
}
