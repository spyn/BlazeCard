using System.Text.Json.Serialization;

namespace BlazeCard.Models.Google;

public class BarcodeDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("alternateText")]
    public string? AlternateText { get; set; }
}
