using System.Text.Json.Serialization;

namespace BlazeCard.Models.Google;

public class ImageDto
{
    [JsonPropertyName("sourceUri")]
    public ImageUriDto SourceUri { get; set; } = new();
}

public class ImageUriDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
}
