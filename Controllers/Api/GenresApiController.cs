using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers.Api;

[ApiController]
[Route("api/genres")]
public class GenresApiController : ControllerBase
{
    private readonly GenresRepository _genresRepository;

    public GenresApiController(GenresRepository genresRepository)
    {
        _genresRepository = genresRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<GenreDto>> Get([FromQuery] string? query)
    {
        var genres = string.IsNullOrWhiteSpace(query)
            ? _genresRepository.GetAll()
            : _genresRepository.Search(query);

        return Ok(genres.Select(ToDto));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public ActionResult<GenreDto> GetById(int id)
    {
        var genre = _genresRepository.GetById(id);
        if (genre == null)
        {
            return NotFound();
        }

        return Ok(ToDto(genre));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<GenreDto> Create([FromBody] GenreCreateDto dto)
    {
        var genre = new Genre
        {
            Name = dto.Name?.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim()
        };

        _genresRepository.Create(genre);
        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, ToDto(genre));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public ActionResult<GenreDto> Update(int id, [FromBody] GenreUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var existing = _genresRepository.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        var genre = new Genre
        {
            Id = id,
            Name = dto.Name?.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim()
        };

        _genresRepository.Update(genre);
        return Ok(ToDto(genre));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_genresRepository.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    private static GenreDto ToDto(Genre genre)
    {
        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            Description = genre.Description
        };
    }
}
