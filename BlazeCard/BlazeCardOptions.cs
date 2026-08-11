namespace BlazeCard;

public class BlazeCardOptions
{
    public int PreviewDebounceMs { get; set; } = 100;
    public int SnippetEmitDebounceMs { get; set; } = 200;
    public int CodeParseDebounceMs { get; set; } = 500;
    public long MaxImageSizeBytes { get; set; } = 5_242_880;
    public string DefaultLanguage { get; set; } = "en-AU";
    public string SessionStorageKey { get; set; } = "blazecard.session.v1";
    public PkPassOptions PkPass { get; set; } = new();
}

public class PkPassOptions
{
    /// <summary>When false, Final tab skips packaging.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// When true (default), Final builds an inspectable .pkpass-shaped zip without Apple signing certs.
    /// When false, requires real Pass Type ID + WWDR certs and uses PassGenerator.Generate().
    /// </summary>
    public bool UsePreviewCertificates { get; set; } = true;

    public string PassTypeIdentifier { get; set; } = "pass.com.example.blazecard";
    public string TeamIdentifier { get; set; } = "TEAMID";

    /// <summary>Only used when UsePreviewCertificates is false.</summary>
    public string PassCertificatePath { get; set; } = "certs/pass.p12";

    /// <summary>Only used when UsePreviewCertificates is false.</summary>
    public string PassCertificatePassword { get; set; } = "";

    /// <summary>Only used when UsePreviewCertificates is false.</summary>
    public string AppleWwdrCertificatePath { get; set; } = "certs/AppleWWDRCAG4.cer";
}
