using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers.Api;

[ApiController]
[Route("api/games")]
public class GamesApiController : ControllerBase
{
    private readonly GamesRepository _gamesRepository;
    private readonly ILogger<GamesApiController> _logger;

    public GamesApiController(GamesRepository gamesRepository, ILogger<GamesApiController> logger)
    {
        _gamesRepository = gamesRepository;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<GameDto>> Get([FromQuery] string? query)
    {
        var games = string.IsNullOrWhiteSpace(query)
            ? _gamesRepository.GetAll()
            : _gamesRepository.Search(query);

        return Ok(games.Select(ToDto));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public ActionResult<GameDto> GetById(int id)
    {
        var game = _gamesRepository.GetById(id);
        if (game == null)
        {
            return NotFound();
        }

        return Ok(ToDto(game));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<GameDto> Create([FromBody] GameCreateDto dto)
    {
        var game = new Game
        {
            Title = dto.Title?.Trim(),
            ReleaseYear = dto.ReleaseYear,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DeveloperId = dto.DeveloperId!.Value,
            AverageRating = dto.AverageRating,
            Genres = dto.GenreIds.Select(id => new Genre { Id = id }).ToList(),
            Platforms = dto.PlatformIds.Select(id => new Platform { Id = id }).ToList()
        };

        _gamesRepository.Create(game);
        _logger.LogInformation("API game created. GameId={GameId}, Title={Title}", game.Id, game.Title);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, ToDto(game));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public ActionResult<GameDto> Update(int id, [FromBody] GameUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var existing = _gamesRepository.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        var game = new Game
        {
            Id = id,
            Title = dto.Title?.Trim(),
            ReleaseYear = dto.ReleaseYear,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            DeveloperId = dto.DeveloperId!.Value,
            AverageRating = dto.AverageRating,
            Genres = dto.GenreIds.Select(genreId => new Genre { Id = genreId }).ToList(),
            Platforms = dto.PlatformIds.Select(platformId => new Platform { Id = platformId }).ToList()
        };

        _gamesRepository.Update(game);
        _logger.LogInformation("API game updated. GameId={GameId}, Title={Title}", game.Id, game.Title);
        return Ok(ToDto(game));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_gamesRepository.Delete(id))
        {
            _logger.LogWarning("API game delete failed. GameId={GameId}", id);
            return NotFound();
        }

        _logger.LogInformation("API game deleted. GameId={GameId}", id);
        return NoContent();
    }

    private static GameDto ToDto(Game game)
    {
        return new GameDto
        {
            Id = game.Id,
            Title = game.Title,
            ReleaseYear = game.ReleaseYear,
            Description = game.Description,
            DeveloperId = game.DeveloperId,
            Developer = game.Developer == null
                ? null
                : new DeveloperSummaryDto
                {
                    Id = game.Developer.Id,
                    Name = game.Developer.Name
                },
            AverageRating = game.AverageRating,
            Genres = game.Genres.Select(genre => new GenreSummaryDto
            {
                Id = genre.Id,
                Name = genre.Name
            }).ToList(),
            Platforms = game.Platforms.Select(platform => new PlatformSummaryDto
            {
                Id = platform.Id,
                Name = platform.Name,
                Type = platform.Type.ToString()
            }).ToList()
        };
    }
}
