using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers.Api;

[ApiController]
[Route("api/developers")]
public class DevelopersApiController : ControllerBase
{
    private readonly DevelopersRepository _developersRepository;

    public DevelopersApiController(DevelopersRepository developersRepository)
    {
        _developersRepository = developersRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<DeveloperDto>> Get([FromQuery] string? query)
    {
        var developers = string.IsNullOrWhiteSpace(query)
            ? _developersRepository.GetAll()
            : _developersRepository.Search(query);

        return Ok(developers.Select(ToDto));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public ActionResult<DeveloperDto> GetById(int id)
    {
        var developer = _developersRepository.GetById(id);
        if (developer == null)
        {
            return NotFound();
        }

        return Ok(ToDto(developer));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<DeveloperDto> Create([FromBody] DeveloperCreateDto dto)
    {
        var developer = new Developer
        {
            Name = dto.Name?.Trim(),
            Country = dto.Country?.Trim(),
            Founded = dto.Founded,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim()
        };

        _developersRepository.Create(developer);
        return CreatedAtAction(nameof(GetById), new { id = developer.Id }, ToDto(developer));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public ActionResult<DeveloperDto> Update(int id, [FromBody] DeveloperUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var existing = _developersRepository.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        var developer = new Developer
        {
            Id = id,
            Name = dto.Name?.Trim(),
            Country = dto.Country?.Trim(),
            Founded = dto.Founded,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim()
        };

        _developersRepository.Update(developer);
        return Ok(ToDto(developer));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_developersRepository.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    private static DeveloperDto ToDto(Developer developer)
    {
        return new DeveloperDto
        {
            Id = developer.Id,
            Name = developer.Name,
            Country = developer.Country,
            Founded = developer.Founded,
            Description = developer.Description
        };
    }
}
