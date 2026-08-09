using System.Text.Json.Serialization;

namespace BlazeCard.Models.Google;

public class ImageModuleDataDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "IMAGE_MODULE_ID";

    [JsonPropertyName("mainImage")]
    public ImageDto? MainImage { get; set; }
}
