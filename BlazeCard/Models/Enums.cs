namespace BlazeCard.Models;

public enum AppMode { Form, Code, Visual }

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

public enum ImageSlot
{
    Logo,
    Icon,
    Strip,
    Hero,
    Thumbnail
}

public enum FieldGroup
{
    Header, Primary, Secondary, Auxiliary, Back
}

public enum Skin
{
    Hbf,
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

public enum ToolboxItemType
{
    HeaderField,
    PrimaryField,
    SecondaryField,
    AuxiliaryField,
    BackField,
    LogoImage,
    StripImage,
    IconImage,
    HeroImage,
    ThumbnailImage,
    Barcode,
    BackgroundColor,
    LabelColor,
    ForegroundColor
}

public enum DropZoneId
{
    Header,
    Primary,
    Secondary,
    Auxiliary,
    Back,
    Strip,
    Logo,
    Barcode
}
