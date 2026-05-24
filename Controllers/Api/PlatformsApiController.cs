using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers.Api;

[ApiController]
[Route("api/platforms")]
public class PlatformsApiController : ControllerBase
{
    private readonly PlatformsRepository _platformsRepository;

    public PlatformsApiController(PlatformsRepository platformsRepository)
    {
        _platformsRepository = platformsRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<PlatformDto>> Get([FromQuery] string? query, [FromQuery] string? type)
    {
        var platforms = string.IsNullOrWhiteSpace(query)
            ? _platformsRepository.GetAll()
            : _platformsRepository.Search(query);

        if (!string.IsNullOrWhiteSpace(type))
        {
            if (!Enum.TryParse<PlatformType>(type, true, out var parsedType))
            {
                return BadRequest();
            }

            platforms = platforms.Where(p => p.Type == parsedType).ToList();
        }

        return Ok(platforms.Select(ToDto));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public ActionResult<PlatformDto> GetById(int id)
    {
        var platform = _platformsRepository.GetById(id);
        if (platform == null)
        {
            return NotFound();
        }

        return Ok(ToDto(platform));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<PlatformDto> Create([FromBody] PlatformCreateDto dto)
    {
        var platform = new Platform
        {
            Name = dto.Name?.Trim(),
            Type = dto.Type!.Value
        };

        _platformsRepository.Create(platform);
        return CreatedAtAction(nameof(GetById), new { id = platform.Id }, ToDto(platform));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public ActionResult<PlatformDto> Update(int id, [FromBody] PlatformUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var existing = _platformsRepository.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        var platform = new Platform
        {
            Id = id,
            Name = dto.Name?.Trim(),
            Type = dto.Type!.Value
        };

        _platformsRepository.Update(platform);
        return Ok(ToDto(platform));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_platformsRepository.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    private static PlatformDto ToDto(Platform platform)
    {
        return new PlatformDto
        {
            Id = platform.Id,
            Name = platform.Name,
            Type = platform.Type.ToString()
        };
    }
}
