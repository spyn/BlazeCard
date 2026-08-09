namespace BlazeCard.Models.Google;

public class BarcodeDto
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? AlternateText { get; set; }
}
