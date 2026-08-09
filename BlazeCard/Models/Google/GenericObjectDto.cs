using System.Text.Json.Serialization;

namespace BlazeCard.Models.Google;

public class GenericObjectDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("classId")]
    public string ClassId { get; set; } = string.Empty;

    [JsonPropertyName("genericType")]
    public string GenericType { get; set; } = "GENERIC_TYPE_UNSPECIFIED";

    [JsonPropertyName("hexBackgroundColor")]
    public string? HexBackgroundColor { get; set; }

    [JsonPropertyName("cardTitle")]
    public LocalizedStringDto? CardTitle { get; set; }

    [JsonPropertyName("subheader")]
    public LocalizedStringDto? Subheader { get; set; }

    [JsonPropertyName("header")]
    public LocalizedStringDto? Header { get; set; }

    [JsonPropertyName("logo")]
    public ImageDto? Logo { get; set; }

    [JsonPropertyName("wideLogo")]
    public ImageDto? WideLogo { get; set; }

    [JsonPropertyName("heroImage")]
    public ImageDto? HeroImage { get; set; }

    [JsonPropertyName("imageModulesData")]
    public List<ImageModuleDataDto>? ImageModulesData { get; set; }

    [JsonPropertyName("textModulesData")]
    public List<TextModuleDataDto>? TextModulesData { get; set; }

    [JsonPropertyName("barcode")]
    public BarcodeDto? Barcode { get; set; }
}
