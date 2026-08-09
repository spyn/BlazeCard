namespace BlazeCard.Models.Google;

public class ImageDto
{
    public ImageUriDto SourceUri { get; set; } = new();
}

public class ImageUriDto
{
    public string Uri { get; set; } = string.Empty;
}
