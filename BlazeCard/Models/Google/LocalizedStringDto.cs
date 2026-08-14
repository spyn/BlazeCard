using System.Text.Json.Serialization;

namespace BlazeCard.Models.Google;

public class LocalizedStringDto
{
    [JsonPropertyName("defaultValue")]
    public TranslatedStringDto DefaultValue { get; set; } = new();
}

public class TranslatedStringDto
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = "en-AU";

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
