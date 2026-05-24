using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameTracker.Data;
using VideoGameTracker.Dtos;
using VideoGameTracker.Models;

namespace VideoGameTracker.Controllers.Api;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/users")]
public class UsersApiController : ControllerBase
{
    private readonly VideoGameTrackerDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UsersApiController(VideoGameTrackerDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserSummaryDto>> Get([FromQuery] string? query)
    {
        var usersQuery = _dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            usersQuery = usersQuery.Where(u =>
                EF.Functions.Like(u.UserName ?? string.Empty, term) ||
                EF.Functions.Like(u.Email ?? string.Empty, term));
        }

        var users = usersQuery
            .OrderBy(u => u.UserName)
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            })
            .ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public ActionResult<UserDetailDto> GetById(string id)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserDetailDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            OIB = user.OIB,
            JMBG = user.JMBG,
            RegisteredAt = user.RegisteredAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> Create([FromBody] UserCreateDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            OIB = dto.OIB?.Trim() ?? string.Empty,
            JMBG = dto.JMBG?.Trim() ?? string.Empty,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password!);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        await _userManager.AddToRoleAsync(user, "Player");

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new UserDetailDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            OIB = user.OIB,
            JMBG = user.JMBG,
            RegisteredAt = user.RegisteredAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDetailDto>> Update(string id, [FromBody] UserUpdateDto dto)
    {
        if (!string.Equals(id, dto.Id, StringComparison.Ordinal))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(dto.UserName))
        {
            user.UserName = dto.UserName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            user.Email = dto.Email.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.OIB))
        {
            user.OIB = dto.OIB.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.JMBG))
        {
            user.JMBG = dto.JMBG.Trim();
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem(ModelState);
            }
        }

        return Ok(new UserDetailDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            OIB = user.OIB,
            JMBG = user.JMBG,
            RegisteredAt = user.RegisteredAt
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
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
            return BadRequest();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return NoContent();
    }
}
