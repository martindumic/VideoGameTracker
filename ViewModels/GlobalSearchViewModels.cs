using System.Linq;

namespace VideoGameTracker.ViewModels;

public class GlobalSearchViewModel
{
    public string? Query { get; set; }

    public List<GlobalSearchGroupViewModel> Groups { get; set; } = new();

    public int TotalCount => Groups.Sum(group => group.Items.Count);

    public bool HasResults => Groups.Any(group => group.Items.Count > 0);
}

public class GlobalSearchGroupViewModel
{
    public string Title { get; set; } = string.Empty;

    public List<GlobalSearchItemViewModel> Items { get; set; } = new();
}

public class GlobalSearchItemViewModel
{
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}
