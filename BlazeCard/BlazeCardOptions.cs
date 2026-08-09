namespace BlazeCard;

public class BlazeCardOptions
{
    public int PreviewDebounceMs { get; set; } = 100;
    public int SnippetEmitDebounceMs { get; set; } = 200;
    public int CodeParseDebounceMs { get; set; } = 500;
    public long MaxImageSizeBytes { get; set; } = 5_242_880;
    public string DefaultLanguage { get; set; } = "en-AU";
    public string SessionStorageKey { get; set; } = "blazecard.session.v1";
}
