namespace VideoGameTracker.ViewModels;

public class DateTimePickerViewModel
{
    public string Label { get; set; } = string.Empty;
    public string InputName { get; set; } = string.Empty;
    public string InputId { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Placeholder { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}
