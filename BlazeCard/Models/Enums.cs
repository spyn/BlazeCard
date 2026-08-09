namespace BlazeCard.Models;

public enum AppMode { Form, Code }

public enum PassType
{
    /// <summary>v1 supports Generic only. Other values are reserved / not offered in UI.</summary>
    Generic = 0
}

public enum BarcodeFormat
{
    None,
    QR,
    PDF417,
    Aztec,
    Code128
}

public enum TextAlignment { Left, Center, Right, Natural }

/// <summary>Google Wallet image slots (HTTPS in snippets; data URIs in preview).</summary>
public enum GoogleImageSlot
{
    Hero,
    WideLogo,
    ImageModule
}

public enum FieldGroup
{
    Header, Primary, Secondary, Auxiliary, Back
}

public enum Skin
{
    Blaze
}

public enum AppearanceMode
{
    Light,
    Dark
}

public enum PreviewFocus
{
    Dual,
    Apple,
    Google
}
