namespace VideoGameTracker.Dtos;

public class DeveloperSummaryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class GenreSummaryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class PlatformSummaryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
}

public class GameSummaryDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
}

public class UserSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
