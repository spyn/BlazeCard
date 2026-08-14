using System.Text.Json.Serialization;

namespace BlazeCard.Models.Google;

public class LinksModuleDataDto
{
    [JsonPropertyName("uris")]
    public List<UriDto> Uris { get; set; } = [];
}

public class UriDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
