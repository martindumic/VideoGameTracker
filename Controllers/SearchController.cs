using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Models;
using VideoGameTracker.ViewModels;

namespace VideoGameTracker.Controllers;

[AllowAnonymous]
[Route("search")]
public class SearchController : BaseController
{
    private readonly GamesRepository _gamesRepository;
    private readonly DevelopersRepository _developersRepository;
    private readonly GenresRepository _genresRepository;
    private readonly PlatformsRepository _platformsRepository;
    private readonly GameEntriesRepository _gameEntriesRepository;

    public SearchController(
        GamesRepository gamesRepository,
        DevelopersRepository developersRepository,
        GenresRepository genresRepository,
        PlatformsRepository platformsRepository,
        GameEntriesRepository gameEntriesRepository,
        UserManager<AppUser> userManager)
        : base(userManager)
    {
        _gamesRepository = gamesRepository;
        _developersRepository = developersRepository;
        _genresRepository = genresRepository;
        _platformsRepository = platformsRepository;
        _gameEntriesRepository = gameEntriesRepository;
    }

    [HttpGet("")]
    public IActionResult Index([FromQuery] string? query)
    {
        var model = BuildSearchViewModel(query, isSuggestion: false);
        return View(model);
    }

    [HttpGet("suggestions")]
    public IActionResult Suggestions([FromQuery] string? query)
    {
        var model = BuildSearchViewModel(query, isSuggestion: true);
        return Json(model);
    }

    private GlobalSearchViewModel BuildSearchViewModel(string? query, bool isSuggestion)
    {
        var trimmed = query?.Trim();
        var model = new GlobalSearchViewModel
        {
            Query = trimmed
        };

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return model;
        }

        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        var isAdmin = IsAdmin;
        var groups = new List<GlobalSearchGroupViewModel>();

        var pageItems = BuildPageItems(trimmed, isAuthenticated, isAdmin);
        if (pageItems.Count > 0)
        {
            groups.Add(new GlobalSearchGroupViewModel
            {
                Title = "Pages",
                Items = pageItems
            });
        }

        var gameLimit = isSuggestion ? 5 : 14;
        var entryLimit = isSuggestion ? 6 : 18;

        var games = _gamesRepository.Search(trimmed)
            .OrderBy(g => g.Title)
            .Take(gameLimit)
            .Select(game => new GlobalSearchItemViewModel
            {
                Title = game.Title ?? "Untitled game",
                Subtitle = game.Developer?.Name,
                Category = "Game",
                Url = Url.Action("Details", "Games", new { id = game.Id }) ?? "/games"
            })
            .ToList();

        if (games.Count > 0)
        {
            groups.Add(new GlobalSearchGroupViewModel
            {
                Title = "Games",
                Items = games
            });
        }

        var entryResults = _gameEntriesRepository.Search(trimmed)
            .OrderByDescending(entry => entry.UserId == CurrentUserId)
            .ThenByDescending(entry => entry.CreatedAt)
            .Take(entryLimit)
            .Select(entry => new GlobalSearchItemViewModel
            {
                Title = entry.Game?.Title ?? "Game entry",
                Subtitle = BuildEntrySubtitle(entry),
                Category = "Game Entry",
                Url = Url.Action("Details", "GameEntries", new { id = entry.Id }) ?? "/game-entries"
            })
            .ToList();

        if (entryResults.Count > 0)
        {
            groups.Add(new GlobalSearchGroupViewModel
            {
                Title = "Game Entries",
                Items = entryResults
            });
        }

        if (isAdmin)
        {
            var developers = _developersRepository.Search(trimmed)
                .OrderBy(d => d.Name)
                .Take(gameLimit)
                .Select(dev => new GlobalSearchItemViewModel
                {
                    Title = dev.Name ?? "Developer",
                    Subtitle = dev.Country,
                    Category = "Developer",
                    Url = Url.Action("Details", "Developers", new { id = dev.Id }) ?? "/developers"
                })
                .ToList();

            if (developers.Count > 0)
            {
                groups.Add(new GlobalSearchGroupViewModel
                {
                    Title = "Developers",
                    Items = developers
                });
            }

            var genres = _genresRepository.Search(trimmed)
                .OrderBy(g => g.Name)
                .Take(gameLimit)
                .Select(genre => new GlobalSearchItemViewModel
                {
                    Title = genre.Name ?? "Genre",
                    Subtitle = genre.Description,
                    Category = "Genre",
                    Url = Url.Action("Details", "Genres", new { id = genre.Id }) ?? "/genres"
                })
                .ToList();

            if (genres.Count > 0)
            {
                groups.Add(new GlobalSearchGroupViewModel
                {
                    Title = "Genres",
                    Items = genres
                });
            }

            var platforms = _platformsRepository.Search(trimmed)
                .OrderBy(p => p.Name)
                .Take(gameLimit)
                .Select(platform => new GlobalSearchItemViewModel
                {
                    Title = platform.Name ?? "Platform",
                    Subtitle = platform.Type.ToString(),
                    Category = "Platform",
                    Url = Url.Action("Details", "Platforms", new { id = platform.Id }) ?? "/platforms"
                })
                .ToList();

            if (platforms.Count > 0)
            {
                groups.Add(new GlobalSearchGroupViewModel
                {
                    Title = "Platforms",
                    Items = platforms
                });
            }
        }

        model.Groups = groups;
        return model;
    }

    private List<GlobalSearchItemViewModel> BuildPageItems(string query, bool isAuthenticated, bool isAdmin)
    {
        var pages = new List<GlobalSearchItemViewModel>
        {
            new()
            {
                Title = "Home",
                Subtitle = "Start page",
                Category = "Page",
                Url = Url.Action("Index", "Home") ?? "/"
            },
            new()
            {
                Title = "Games",
                Subtitle = "Browse the catalogue",
                Category = "Page",
                Url = Url.Action("Index", "Games") ?? "/games"
            },
            new()
            {
                Title = "Game Entries",
                Subtitle = "All community entries",
                Category = "Page",
                Url = Url.Action("Index", "GameEntries") ?? "/game-entries"
            },
            new()
            {
                Title = "Privacy",
                Subtitle = "Privacy policy",
                Category = "Page",
                Url = Url.Action("Privacy", "Home") ?? "/home/privacy"
            }
        };

        if (isAuthenticated)
        {
            pages.Add(new GlobalSearchItemViewModel
            {
                Title = "My Entries",
                Subtitle = "Your game entries",
                Category = "Page",
                Url = Url.Action("Index", "GameEntries") ?? "/game-entries"
            });

            var profileUrl = Url.Page("/Account/Manage/Index", new { area = "Identity" }) ?? "/Identity/Account/Manage";
            pages.Add(new GlobalSearchItemViewModel
            {
                Title = "Profile",
                Subtitle = "Account settings",
                Category = "Page",
                Url = profileUrl
            });
        }
        else
        {
            var loginUrl = Url.Page("/Account/Login", new { area = "Identity" }) ?? "/Identity/Account/Login";
            var registerUrl = Url.Page("/Account/Register", new { area = "Identity" }) ?? "/Identity/Account/Register";
            pages.Add(new GlobalSearchItemViewModel
            {
                Title = "Login",
                Subtitle = "Sign in to your account",
                Category = "Page",
                Url = loginUrl
            });
            pages.Add(new GlobalSearchItemViewModel
            {
                Title = "Register",
                Subtitle = "Create a new account",
                Category = "Page",
                Url = registerUrl
            });
        }

        if (isAdmin)
        {
            pages.AddRange(new[]
            {
                new GlobalSearchItemViewModel
                {
                    Title = "Developers",
                    Subtitle = "Admin: manage developers",
                    Category = "Page",
                    Url = Url.Action("Index", "Developers") ?? "/developers"
                },
                new GlobalSearchItemViewModel
                {
                    Title = "Genres",
                    Subtitle = "Admin: manage genres",
                    Category = "Page",
                    Url = Url.Action("Index", "Genres") ?? "/genres"
                },
                new GlobalSearchItemViewModel
                {
                    Title = "Platforms",
                    Subtitle = "Admin: manage platforms",
                    Category = "Page",
                    Url = Url.Action("Index", "Platforms") ?? "/platforms"
                },
                new GlobalSearchItemViewModel
                {
                    Title = "Users",
                    Subtitle = "Admin: manage users",
                    Category = "Page",
                    Url = Url.Action("Index", "Users") ?? "/users"
                }
            });
        }

        return pages
            .Where(page => MatchesQuery(query, page.Title, page.Subtitle))
            .OrderBy(page => page.Title)
            .ToList();
    }

    private static bool MatchesQuery(string query, string title, string? subtitle)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return false;
        }

        var comparison = StringComparison.OrdinalIgnoreCase;
        if (title.Contains(query, comparison))
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(subtitle) && subtitle.Contains(query, comparison);
    }

    private static string BuildEntrySubtitle(GameEntry entry)
    {
        var username = entry.User?.UserName ?? "Unknown";
        var status = entry.Status.ToString();
        var date = entry.CreatedAt.ToString("d", CultureInfo.CurrentCulture);
        return $"{username} • {status} • {date}";
    }
}
