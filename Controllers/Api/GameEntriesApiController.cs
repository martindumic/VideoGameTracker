using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers.Api;

[ApiController]
[Route("api/gameentries")]
public class GameEntriesApiController : ControllerBase
{
    private readonly GameEntriesRepository _gameEntriesRepository;
    private readonly GamesRepository _gamesRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<GameEntriesApiController> _logger;

    public GameEntriesApiController(
        GameEntriesRepository gameEntriesRepository,
        GamesRepository gamesRepository,
        UserManager<AppUser> userManager,
        ILogger<GameEntriesApiController> logger)
    {
        _gameEntriesRepository = gameEntriesRepository;
        _gamesRepository = gamesRepository;
        _userManager = userManager;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<GameEntryDto>> Get([FromQuery] string? query, [FromQuery] string? status)
    {
        var entries = string.IsNullOrWhiteSpace(query)
            ? _gameEntriesRepository.GetAll()
            : _gameEntriesRepository.Search(query);

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<GameStatus>(status, true, out var parsedStatus))
            {
                return BadRequest();
            }

            entries = entries.Where(e => e.Status == parsedStatus).ToList();
        }

        return Ok(entries.Select(ToDto));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public ActionResult<GameEntryDto> GetById(int id)
    {
        var entry = _gameEntriesRepository.GetById(id);
        if (entry == null)
        {
            return NotFound();
        }

        return Ok(ToDto(entry));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<GameEntryDto>> Create([FromBody] GameEntryCreateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("API game entry create blocked. Missing user id.");
            return Challenge();
        }

        if (dto.GameId == null)
        {
            return BadRequest();
        }

        var game = _gamesRepository.GetById(dto.GameId.Value);
        if (game == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole("Admin");
        var entryUserId = userId;

        if (isAdmin && !string.IsNullOrWhiteSpace(dto.UserId))
        {
            var targetUser = await _userManager.FindByIdAsync(dto.UserId);
            if (targetUser == null)
            {
                return NotFound();
            }

            entryUserId = targetUser.Id;
        }

        var entry = new GameEntry
        {
            GameId = dto.GameId.Value,
            UserId = entryUserId,
            Status = dto.Status!.Value,
            HoursPlayed = dto.HoursPlayed,
            ReviewScore = dto.ReviewScore,
            ReviewComment = string.IsNullOrWhiteSpace(dto.ReviewComment) ? null : dto.ReviewComment.Trim(),
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
        };

        _gameEntriesRepository.Create(entry);
        _logger.LogInformation(
            "API game entry created. EntryId={EntryId}, UserId={UserId}, GameId={GameId}",
            entry.Id,
            entry.UserId,
            entry.GameId);
        var refreshed = _gameEntriesRepository.GetById(entry.Id) ?? entry;
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, ToDto(refreshed));
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<GameEntryDto>> Update(int id, [FromBody] GameEntryUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var entry = _gameEntriesRepository.GetById(id);
        if (entry == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && entry.UserId != userId)
        {
            _logger.LogWarning("API game entry update forbidden. EntryId={EntryId}, UserId={UserId}", entry.Id, userId);
            return Forbid();
        }

        if (dto.GameId == null)
        {
            return BadRequest();
        }

        var game = _gamesRepository.GetById(dto.GameId.Value);
        if (game == null)
        {
            return NotFound();
        }

        var entryUserId = entry.UserId;
        if (isAdmin && !string.IsNullOrWhiteSpace(dto.UserId))
        {
            var targetUser = await _userManager.FindByIdAsync(dto.UserId);
            if (targetUser == null)
            {
                return NotFound();
            }

            entryUserId = targetUser.Id;
        }

        var updated = new GameEntry
        {
            Id = id,
            GameId = dto.GameId.Value,
            UserId = entryUserId,
            Status = dto.Status!.Value,
            HoursPlayed = dto.HoursPlayed,
            ReviewScore = dto.ReviewScore,
            ReviewComment = string.IsNullOrWhiteSpace(dto.ReviewComment) ? null : dto.ReviewComment.Trim(),
            CreatedAt = dto.CreatedAt ?? entry.CreatedAt
        };

        _gameEntriesRepository.Update(updated);
        _logger.LogInformation(
            "API game entry updated. EntryId={EntryId}, UserId={UserId}, GameId={GameId}",
            updated.Id,
            updated.UserId,
            updated.GameId);
        var refreshed = _gameEntriesRepository.GetById(id) ?? updated;
        return Ok(ToDto(refreshed));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var entry = _gameEntriesRepository.GetById(id);
        if (entry == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && entry.UserId != userId)
        {
            _logger.LogWarning("API game entry delete forbidden. EntryId={EntryId}, UserId={UserId}", entry.Id, userId);
            return Forbid();
        }

        _gameEntriesRepository.Delete(id);
        _logger.LogInformation("API game entry deleted. EntryId={EntryId}, UserId={UserId}", id, entry.UserId);
        return NoContent();
    }

    private static GameEntryDto ToDto(GameEntry entry)
    {
        return new GameEntryDto
        {
            Id = entry.Id,
            Game = entry.Game == null ? null : new GameSummaryDto
            {
                Id = entry.Game.Id,
                Title = entry.Game.Title
            },
            User = entry.User == null ? null : new UserSummaryDto
            {
                Id = entry.User.Id,
                UserName = entry.User.UserName,
                Email = entry.User.Email
            },
            Status = entry.Status,
            CreatedAt = entry.CreatedAt,
            HoursPlayed = entry.HoursPlayed,
            ReviewScore = entry.ReviewScore,
            ReviewComment = entry.ReviewComment
        };
    }
}
