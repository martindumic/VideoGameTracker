namespace VideoGameTracker.Dtos;

public class UserDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? OIB { get; set; }
    public string? JMBG { get; set; }
    public DateTime RegisteredAt { get; set; }
}
